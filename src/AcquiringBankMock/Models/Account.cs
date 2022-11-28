namespace AcquiringBankMock.Models;

public class Account
{
    public CardDetails CardDetails { get; init; }
    
    public List<SpecificCurrencyAccount> SpecificCurrencyAccounts { get; set; }
    
    public BankResult Withdraw(string currency, decimal amount)
    {
        var account = SpecificCurrencyAccounts.FirstOrDefault(a => a.Currency == currency);
        
        if(account == null)
            return BankResult.UnsupportedCurrency;
        
        if(!CardDetails.IsValid)
            return BankResult.CardExpired;

        return account.TryWithdraw(amount) ? BankResult.Success : BankResult.NotEnoughMoney;
    }
}