using MediatR;
using PaymentGateway.Core;

namespace PaymentGateway.Handlers;

public class PaymentRequestAcceptedEventHandler : INotificationHandler<Payment>
{
    private readonly IPaymentProcessor _paymentProcessor;
    
    public PaymentRequestAcceptedEventHandler(IPaymentProcessor paymentProcessor)
    {
        _paymentProcessor = paymentProcessor;
    }
    
    public async Task Handle(Payment notification, CancellationToken cancellationToken)
    {
        // TODO: retry, handle errors
        await _paymentProcessor.Process(notification);
    }
}