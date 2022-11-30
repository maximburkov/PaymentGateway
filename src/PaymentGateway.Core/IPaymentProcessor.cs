namespace PaymentGateway.Core;

public interface IPaymentProcessor
{
    public Task<bool> Process(Payment payment);
}