using FluentValidation;
using PaymentGateway.Dto;

namespace PaymentGateway.Validators;

public class PaymentRequestValidator : AbstractValidator<PaymentRequest>
{
    public PaymentRequestValidator()
    {
        RuleFor(payment => payment.CardNumber)
            .CreditCard()
            .WithMessage("Invalid card number.");

        RuleFor(payment => payment.Amount)
            .GreaterThan(0);
        
        // TODO: Add other checks
    }

}