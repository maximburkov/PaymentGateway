namespace PaymentGateway.Core;

public class PaymentProcessor :  IPaymentProcessor
{
    private readonly IAcquiringBankService _bankService;
    
    public PaymentProcessor(IAcquiringBankService bankService)
    {
        _bankService = bankService;
    }
    
    public async Task<bool> Process(Payment payment)
    {
        var result = await _bankService.MakePayment(payment);
        payment.Status = result.IsSuccesseful ? Status.Succeeded : Status.Failed;
        if (!result.IsSuccesseful)
            payment.RejectionReason = result.Error;
        return result.IsSuccesseful;
    }
}