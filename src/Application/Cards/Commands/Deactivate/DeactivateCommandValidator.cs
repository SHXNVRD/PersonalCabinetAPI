using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Cards.Commands.Deactivate
{
    public class DeactivateCommandValidator : AbstractValidator<DeactivateCardCommand>
    {
        public DeactivateCommandValidator()
        {
            // RuleFor(c => c.CardNumber)
            //     .NotEmpty().WithMessage("Card number is required");
        }
    }
}