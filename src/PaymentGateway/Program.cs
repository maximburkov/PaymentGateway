using Microsoft.AspNetCore.Http.HttpResults;
using PaymentGateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/payment", () =>
{
    return "ohh of payments";
});
app.MapPost("/payment", (PaymentRequest payment) => Results.Ok(42));

app.UseSwagger();
app.UseSwaggerUI();

app.Run();