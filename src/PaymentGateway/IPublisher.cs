namespace PaymentGateway;

public interface IPublisher
{
    Task Send<TNotification>(TNotification notification, CancellationToken token = default);
}