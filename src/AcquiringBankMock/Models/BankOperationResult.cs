namespace AcquiringBankMock.Models;

public record BankOperationResult(bool IsSuccessful, string? RejectionReason = null)
{
    public static BankOperationResult Success => new (true);

    public static BankOperationResult NotEnoughMoney => new (false, "Not enough money for operation.");

    public static BankOperationResult InvalidCredentials => new (false, "Invalid credentials.");

    public static BankOperationResult UnsupportedCurrency => new (false, "Currency not supported.");

    public static BankOperationResult CardExpired => new BankOperationResult(false, "Card expired.");
}