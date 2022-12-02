using AcquiringBankMock.Models;

namespace AcquiringBankMock;

// Class with mock in memory data.
public static class DataInitializer
{
    public static Bank CreateBankMock()
    {
        var accounts = new List<Account>
        {
            new()
            {
                CardDetails = new CardDetails("3742454554001261", "Elon Musk", "777", 2025, 12),
                SpecificCurrencyAccounts = new List<SpecificCurrencyAccount>
                {
                    SpecificCurrencyAccount.Create("USD", 100_000_000_000),
                    SpecificCurrencyAccount.Create("GBP", 1000)
                }
            },
            new()
            {
                CardDetails = new CardDetails("3742454554001211", "Harry Potter", "888", 2024, 4),
                SpecificCurrencyAccounts = new List<SpecificCurrencyAccount>
                {
                    SpecificCurrencyAccount.Create("GBP", 100)
                }
            },
            new()
            {
                CardDetails = new CardDetails("3742454554001221", "Empty Dollar", "880", 2022, 4),
                SpecificCurrencyAccounts = new List<SpecificCurrencyAccount>
                {
                    SpecificCurrencyAccount.CreateEmpty("USD")
                }
            }
        };

        return new Bank(accounts);
    }
}