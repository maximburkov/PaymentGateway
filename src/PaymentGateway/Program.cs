using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PaymentGateway;
using PaymentGateway.Core;
using PaymentGateway.Endpoints;
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

builder.Services.AddSingleton<IPublisher, BackgroundPublisher>();
builder.Services.AddSingleton<IPaymentProcessor, PaymentProcessor>();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// TODO: move to extension
var contact = new OpenApiContact()
{
    Name = "FirstName LastName",
    Email = "user@example.com",
    Url = new Uri("http://www.example.com")
};

var license = new OpenApiLicense()
{
    Name = "My License",
    Url = new Uri("http://www.example.com")
};

var info = new OpenApiInfo()
{
    Version = "v1",
    Title = "Swagger Demo Minimal API",
    Description = "Swagger Demo Minimal API Description",
    TermsOfService = new Uri("http://www.example.com"),
    Contact = contact,
    License = license
};

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    o.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
        await context.Response.WriteAsync("An exception was thrown.");
        // TODO: logs
    });
});

app.UsePaymentEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

// Seed Database: TODO: move to extension
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.Run();