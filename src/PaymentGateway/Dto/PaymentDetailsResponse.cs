using PaymentGateway.Core;

namespace PaymentGateway.Dto;

public record PaymentDetailsResponse(
    Guid Id, 
    Guid MerchantId,
    string CardNumber, 
    string Name,
    string Cvv,
    string Currency,
    decimal Amount, 
    string Status,
    string? RejectionReason)
{
    public static PaymentDetailsResponse FromPayment(Payment payment) => new(
        payment.Id,
        payment.MerchantId,
        payment.CardNumber.MaskCardNumber('*'),
        payment.Name.MaskName('*'),
        payment.Cvv.MaskCvv('*'),
        payment.Currency,
        payment.Amount,
        payment.Status.ToString(),
        payment.RejectionReason
    );
}