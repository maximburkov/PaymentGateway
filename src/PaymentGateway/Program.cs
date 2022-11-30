using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentGateway;
using PaymentGateway.Core;
using PaymentGateway.Dto;
using PaymentGateway.Infrastructure.Integration;
using PaymentGateway.Infrastructure.Persistence;
using IPublisher = PaymentGateway.IPublisher;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IAcquiringBankService, AcquiringBankMockService>();
builder.Services.AddHttpClient<IAcquiringBankService, AcquiringBankMockService>(client =>
{
    client.BaseAddress =
        new Uri(Environment.GetEnvironmentVariable("BANK_API_URL") ?? throw new ArgumentNullException(""));
});

var s = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddSingleton<IPublisher, BackgroundPublisher>();
builder.Services.AddSingleton<IPaymentProcessor, PaymentProcessor>();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        await context.Response.WriteAsync("An exception was thrown.");
    });
});

app.MapGet("/payment", async ([FromServices]ApplicationDbContext context) =>
{
    var payments = await context.Payments
        .AsNoTracking()
        .ToListAsync();
    
    return Results.Ok(payments);
});

app.MapPost("/payment", async (PaymentRequest paymentRequest,
    [FromServices]ApplicationDbContext context,
    [FromServices]IPublisher publisher,
    IValidator<PaymentRequest> validator) =>
{
    var validationResult = validator.Validate(paymentRequest);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors);
    }
    
    var payment = paymentRequest.AsPayment();
    payment.Status = Status.Pending;
    context.Payments.Add(payment);
    await context.SaveChangesAsync();
    
    await publisher.Send(payment);
    
    return Results.Accepted($"/payment/{payment.Id}", payment);
});

app.UseSwagger();
app.UseSwaggerUI();

// Seed Database: TODO: move to extension
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.Run();