namespace AcquiringBankMock.Models;

public record BankResult(string? RejectionReason)
{
    public static BankResult Success => new ((string?)null);

    public static BankResult NotEnoughMoney => new ("Not enough money for operation.");

    public static BankResult InvalidCredentials => new ("Invalid credentials.");

    public static BankResult UnsupportedCurrency => new ("Currency not supported.");

    public static BankResult CardExpired => new BankResult("Card expired.");
}