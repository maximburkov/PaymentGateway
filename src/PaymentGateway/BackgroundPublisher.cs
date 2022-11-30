using MediatR;

namespace PaymentGateway;

// TODO: naming
public class BackgroundPublisher : IPublisher
{
    private class BackgroundMediator : Mediator
    {
        public BackgroundMediator(ServiceFactory serviceFactory) : base(serviceFactory)
        {
        }

        protected override Task PublishCore(IEnumerable<Func<INotification, CancellationToken, Task>> allHandlers, INotification notification, CancellationToken cancellationToken)
        {
            foreach (var handler in allHandlers)
            {
                Task.Run(() => handler(notification, cancellationToken), cancellationToken);
            }

            return Task.CompletedTask;
        }
    }

    private readonly BackgroundMediator _mediator;

    public BackgroundPublisher(ServiceFactory serviceFactory)
    {
        _mediator = new BackgroundMediator(serviceFactory);
    }

    public Task Send<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : notnull
    {
        return _mediator.Publish(notification, cancellationToken);
    }
}