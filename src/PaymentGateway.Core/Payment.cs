using MediatR;

namespace PaymentGateway.Core;

public enum Status{
    Pending,
    Rejected,
    Verified // TODO: naming
}

public class Payment : INotification
{
    public Guid Id { get; set; }
    
    public string CardNumber { get; set; } 
    
    public string Name { get; set; }
    
    public int Amount { get; set; }
    
    public Status Status { get; set; }

    // TODO: add Currency 
}