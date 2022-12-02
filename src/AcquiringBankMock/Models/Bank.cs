namespace AcquiringBankMock.Models;

// TODO: change Models folder
public class Bank
{
    private readonly List<Account> _accounts;

    public Bank(List<Account> accounts)
    {
        _accounts = accounts;
    }

    public BankOperationResult MakePayment(CardDetails cardDetails, string currency, decimal amount)
    {
        var account = _accounts.FirstOrDefault(a => a.CardDetails == cardDetails);
        return account == null ? BankOperationResult.InvalidCredentials : account.Withdraw(currency, amount);
    }
}