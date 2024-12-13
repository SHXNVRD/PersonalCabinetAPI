using Application.Cards.Commands.Activate;
using FluentValidation;

namespace API.DTOs.CardController
{
    public class ActivateCardRequestValidator : AbstractValidator<ActivateCardRequest>
    {
        public ActivateCardRequestValidator()
        {
            RuleFor(r => r.CardCode)
                .NotEmpty().WithMessage("Card code is required")
                .Length(6).WithMessage("Invalid card code");

            RuleFor(r => r.CardNumber)
                .NotEmpty().WithMessage("Card number is required");
        }
    }
}