using PaymentGateway.Core;

namespace PaymentGateway.Dto;

public record CardDetails
{
    /// <example>3742454554001211</example>
    public string Number { get; init; }
    
    /// <example>Harry Potter</example>
    public string Name { get; init; } 
    
    /// <example>888</example>
    public string Cvv { get; init; }
    
    /// <example>2024</example>
    public int ExpYear { get; init; }
    
    /// <example>4</example>
    public int ExpMonth { get; init; }
}

public record PaymentRequest
{
    public Guid IdempotencyKey { get; init; }
     public CardDetails CardDetails { get; init; }
     
     /// <example>10</example>
     public decimal Amount { get; init; }
     
     /// <example>GBP</example>
     public string Currency { get; init; }
     
     public Payment AsPayment()
     {
         return Payment.Initiate(IdempotencyKey, CardDetails.Number, CardDetails.Name, Currency, Amount,
             CardDetails.ExpYear, CardDetails.ExpMonth, CardDetails.Cvv);
     }
}