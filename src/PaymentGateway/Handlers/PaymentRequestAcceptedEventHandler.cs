using MediatR;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Core;
using PaymentGateway.Infrastructure.Persistence;

namespace PaymentGateway.Handlers;

public class PaymentRequestAcceptedEventHandler : INotificationHandler<Payment>
{
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly ApplicationDbContext _dbContext;
    public PaymentRequestAcceptedEventHandler(IPaymentProcessor paymentProcessor, ApplicationDbContext dbContext)
    {
        _paymentProcessor = paymentProcessor;
        _dbContext = dbContext;
    }
    
    public async Task Handle(Payment notification, CancellationToken cancellationToken)
    {
        // TODO: retry, handle errors
        _dbContext.Attach(notification);
        await _paymentProcessor.Process(notification);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}