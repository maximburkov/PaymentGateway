namespace PaymentGateway.Core;

public interface IAcquiringBankService
{
    // TODO: replace with not bool
    Task<bool> MakePayment(Payment payment);
}