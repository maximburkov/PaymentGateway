using AcquiringBankMock;
using AcquiringBankMock.Dto;
using AcquiringBankMock.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(_ => CreateBankMock());

var app = builder.Build();

app.MapGet("/payment", () =>
{
    return Results.Ok(new PaymentResponse(true, "Test"));
});

app.MapPost("/payment", (PaymentRequest payment, [FromServices]Bank bank) =>
{
    var result = bank.MakePayment(new CardDetails(payment.CardNumber, payment.Name, payment.Cvv, payment.ExpYear, payment.ExpMonth),
        payment.Currency, payment.Amount);
    
    return Results.Ok(new PaymentResponse(result == BankResult.Success, result.RejectionReason));
    // TODO: think about other return codes
});

app.Run();

// TODO: move to another file
static Bank CreateBankMock()
{
    var accounts = new List<Account>
    {
        new()
        {
            CardDetails = new CardDetails("374245455400126", "Elon Maks", "777", 2025, 12),
            SpecificCurrencyAccounts = new List<SpecificCurrencyAccount>
            {
                SpecificCurrencyAccount.Create("USD", 2000),
                SpecificCurrencyAccount.Create("GBP", 1000)
            }
        },
        new()
        {
            CardDetails = new CardDetails("374245455400121", "Harry Potter", "888", 2022, 4),
            SpecificCurrencyAccounts = new List<SpecificCurrencyAccount>
            {
                SpecificCurrencyAccount.CreateEmpty("GPB")
            }
        }
    };

    return new Bank(accounts);
}