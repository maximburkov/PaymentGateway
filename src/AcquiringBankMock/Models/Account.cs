namespace AcquiringBankMock.Models;

public class Account
{
    public CardDetails CardDetails { get; init; }
    
    public List<SpecificCurrencyAccount> SpecificCurrencyAccounts { get; set; }
    
    public BankOperationResult Withdraw(string currency, decimal amount)
    {
        var account = SpecificCurrencyAccounts.FirstOrDefault(a => a.Currency == currency);
        
        if(account == null)
            return BankOperationResult.UnsupportedCurrency;
        
        if(!CardDetails.IsValid())
            return BankOperationResult.CardExpired;

        return account.TryWithdraw(amount) ? BankOperationResult.Success : BankOperationResult.NotEnoughMoney;
    }
}