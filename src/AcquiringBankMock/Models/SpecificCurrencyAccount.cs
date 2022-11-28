namespace AcquiringBankMock.Models;

public class SpecificCurrencyAccount
{
    private SpecificCurrencyAccount(string currency, decimal balance)
    {
        Currency = currency;
        Balance = balance;
    }
    
    public decimal Balance { get; set; }
    
    public string Currency { get; }

    public static SpecificCurrencyAccount Create(string currency, decimal balance) => new(currency, balance);

    public static SpecificCurrencyAccount CreateEmpty(string currency) => new(currency, 0);

    public bool TryWithdraw(decimal amount)
    {
        if (amount > Balance) return false;
        
        Balance -= amount;
        return true;
    }
}