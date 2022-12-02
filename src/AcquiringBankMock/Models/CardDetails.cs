namespace AcquiringBankMock.Models;

public record CardDetails(string Number, string HolderName, string Cvv, int ExpirationYear, int ExpirationMonth)
{
    // Should not compare with DateTime.Now. 
    // Just for testing
    public bool IsValid() => DateTime.Today < new DateTime(year: ExpirationYear, month: ExpirationMonth, day: 1).AddMonths(1);
}