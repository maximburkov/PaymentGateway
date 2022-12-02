namespace PaymentGateway.Core;

public static class MaskExtensions
{
    public static string MaskCardNumber(this string cardNumber, char maskingSymbol)
    {
        var maskedPart = CreateMaskedPart(maskingSymbol, cardNumber.Length - 8);
        return $"{cardNumber[..4]}{maskedPart}{cardNumber[^4..]}";
    }
        
    public static string MaskName(this string name, char maskingSymbol)
    {
        int shownCount = 3;
        if (name.Length > shownCount)
            return $"{name[..shownCount]}{CreateMaskedPart(maskingSymbol, name.Length - shownCount)}";

        return CreateMaskedPart(maskingSymbol, shownCount);
    }

    public static string MaskCvv(this string cvv, char maskingSymbol) =>
        $"{CreateMaskedPart(maskingSymbol, cvv.Length - 1)}{cvv[^1..]}";

    private static string CreateMaskedPart(char maskingSymbol, int length) =>
        new (Enumerable.Repeat(maskingSymbol, length).ToArray());
}