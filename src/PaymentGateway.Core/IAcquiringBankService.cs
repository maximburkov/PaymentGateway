namespace PaymentGateway.Core;

public interface IAcquiringBankService
{
    // TODO: replace with not bool
    Task<(bool isSuccesseful, string? error)> MakePayment(Payment payment);
}