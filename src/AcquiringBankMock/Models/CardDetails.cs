namespace AcquiringBankMock.Models;

public record CardDetails
{
    public CardDetails( string number, string holderName, string cvv, int expYear, int expMonth)
    {
        Number = number;
        HolderName = holderName;
        Cvv = cvv;
        ExpirationYear = expYear;
        ExpirationMonth = expMonth;
    }
    
    public string Number { get; init; }
    
    public string HolderName { get; init; }
    
    public string Cvv { get; init; }

    public int ExpirationMonth { get; init; }
    
    public int ExpirationYear { get; init; }
    
    // Should not compare with DateTime.Now. 
    // Just for testing
    public bool IsValid => DateTime.Today < new DateTime(year: ExpirationYear, month: ExpirationMonth, day: 1).AddMonths(1);
}