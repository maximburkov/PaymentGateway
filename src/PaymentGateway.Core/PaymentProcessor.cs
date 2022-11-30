namespace PaymentGateway.Core;

public class PaymentProcessor :  IPaymentProcessor
{
    private readonly IAcquiringBankService _bankService;
    
    public PaymentProcessor(IAcquiringBankService bankService)
    {
        _bankService = bankService;
    }
    
    // TODO: not bool
    public async Task<bool> Process(Payment payment)
    {
        var result = await _bankService.MakePayment(payment);

        if (result)
            payment.Status = Status.Verified;
        
        return result;
    }
}