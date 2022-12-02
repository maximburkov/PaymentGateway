using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PaymentGateway;
using PaymentGateway.Core;
using PaymentGateway.Endpoints;
using PaymentGateway.Extensions;
using PaymentGateway.Infrastructure.Integration;
using PaymentGateway.Infrastructure.Persistence;
using IPublisher = PaymentGateway.IPublisher;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IAcquiringBankService, AcquiringBankMockService>();
builder.Services.AddHttpClient<IAcquiringBankService, AcquiringBankMockService>(client =>
{
    client.BaseAddress =
        new Uri(Environment.GetEnvironmentVariable("BANK_API_URL") ??
                throw new ArgumentNullException("Acquiring Bank Url is not configured."));
});

builder.Services.AddSingleton<IPublisher, BackgroundPublisher>();
builder.Services.AddSingleton<IPaymentProcessor, PaymentProcessor>();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(o =>
{
    // Adding xml support for predefined Example values in Swagger UI
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    o.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseGlobalExceptionHandling();
app.UsePaymentEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

// Create if database not exists
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.Run();