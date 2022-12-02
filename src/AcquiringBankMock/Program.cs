using AcquiringBankMock;
using AcquiringBankMock.Dto;
using AcquiringBankMock.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(_ => DataInitializer.CreateBankMock());

var app = builder.Build();

// TODO: delete
app.MapGet("/payment", () =>
{
    return Results.Ok(new PaymentResponse(true, "Test"));
});

app.MapPost("/payment", (PaymentRequest payment, [FromServices]Bank bank) =>
{
    var result = bank.MakePayment(new CardDetails(payment.CardNumber, payment.Name, payment.Cvv, payment.ExpYear, payment.ExpMonth),
        payment.Currency, payment.Amount);
    
    return Results.Ok(new PaymentResponse(result == BankOperationResult.Success, result.RejectionReason));
    // TODO: think about other return codes
});

app.Run();