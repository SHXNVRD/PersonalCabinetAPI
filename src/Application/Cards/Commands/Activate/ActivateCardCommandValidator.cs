using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Cards.Commands.Activate
{
    public class ActivateCardCommandValidator : AbstractValidator<ActivateCardCommand>
    {
        public ActivateCardCommandValidator()
        {
            RuleFor(c => c.UserId)
                .NotEmpty().WithMessage("User id is required");

            RuleFor(c => c.CardCode)
                .NotEmpty().WithMessage("Card code is required")
                .Length(6).WithMessage("Invalid card code");

            RuleFor(c => c.CardNumber)
                .NotEmpty().WithMessage("Card number is required");
        }
    }
}