namespace PaymentGateway.Core;

public interface IAcquiringBankService
{
    // TODO: replace with not bool
    Task<(bool IsSuccesseful, string? Error)> MakePayment(Payment payment);
}