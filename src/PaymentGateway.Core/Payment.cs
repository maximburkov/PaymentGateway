using MediatR;

namespace PaymentGateway.Core;

public enum Status{
    Pending,
    Failed,
    Succeeded 
}

public class Payment : INotification
{
    public Guid Id { get; set; }
    
    public Guid IdempotencyKey { get; set; }
    
    public string CardNumber { get; set; } 
    
    public string Name { get; set; }
    
    public string Currency { get; set; }
    
    public decimal Amount { get; set; }
    
    public string Cvv { get; set; }
    
    public int ExpYear { get; set; }
    
    public int ExpMonth { get; set; }

    public Status Status { get; set; } = Status.Pending;

    public static Payment Initiate(Guid idempotencyKey, string cardNumber, string name, string currency, decimal amount, int year, int month, string cvv)
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            IdempotencyKey = idempotencyKey,
            CardNumber = cardNumber,
            Name = name,
            Currency = currency,
            Amount = amount,
            ExpYear = year,
            ExpMonth = month,
            Cvv = cvv,
            Status = Status.Pending
        };
    }
}