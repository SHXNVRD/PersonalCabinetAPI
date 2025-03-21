using FluentValidation;

namespace API.Controllers.Card.DTOs;

public class ActivateCardRequestValidator : AbstractValidator<ActivateCardRequest>
{
    public ActivateCardRequestValidator()
    {
        RuleFor(r => r.CardPinCode)
            .NotEmpty().WithMessage("Card pin code is required")
            .Length(4).WithMessage("Invalid card code");

        RuleFor(r => r.CardNumber)
            .NotEmpty().WithMessage("Card number is required");
    }
}