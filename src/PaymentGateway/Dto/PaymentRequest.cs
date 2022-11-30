using PaymentGateway.Core;

namespace PaymentGateway.Dto;

public record PaymentRequest(Guid Id, string CardNumber, string Name, int Amount)
{
    // TODO: mapper?
    public Payment AsPayment()
    {
        return new Payment
        {
            Id = Id,
            Amount = Amount,
            CardNumber = CardNumber,
            Name = Name
        };
    }
}