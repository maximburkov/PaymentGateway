using FluentValidation;
using PaymentGateway.Dto;

namespace PaymentGateway.Validators;

public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
{
    public PaymentRequestValidator()
    {
        RuleFor(payment => payment.CardDetails.Number)
            .Must(BeValidCreditCard)
            .WithMessage("Card number should contain only digits, number of digits should be from 16 to 19.");
        
        RuleFor(payment => payment.CardDetails.Cvv)
            .Length(3,4)
            .WithMessage("Invalid Cvv.");

        RuleFor(payment => payment.CardDetails.ExpMonth)
            .InclusiveBetween(1,12)
            .WithMessage("Invalid Cvv.");

        RuleFor(payment => payment.Amount)
            .GreaterThan(0);

        RuleFor(payment => payment.Currency)
            .Length(3)
            .Must(payment => payment.All(char.IsLetter))
            .WithMessage("Invalid currency format.");
    }
    
    private static bool BeValidCreditCard(string number)
    {
        number = number.Replace("-", "").Replace(" ", "");
        
        // Very naive approach to check Credit Card Number
        return number.Length is >= 16 and <= 19 &&
               number.All(char.IsDigit);
    }
}