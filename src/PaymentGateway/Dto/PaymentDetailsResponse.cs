using PaymentGateway.Core;

namespace PaymentGateway.Dto;

public record PaymentDetailsResponse(
    Guid Id, 
    string CardNumber, 
    string Name, 
    string Currency, 
    decimal Amount, 
    string Status)
{
    public static PaymentDetailsResponse FromPayment(Payment payment) => new(
        payment.Id,
        payment.CardNumber,
        payment.Name,
        payment.Currency,
        payment.Amount,
        payment.Status.ToString()
    );
}