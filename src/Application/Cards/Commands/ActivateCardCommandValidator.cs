using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Cards.Commands
{
    public class ActivateCardCommandValidator : AbstractValidator<ActivateCardCommand>
    {
        public ActivateCardCommandValidator()
        {
            RuleFor(c => c.UserEmail)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(c => c.CardCode)
                .Length(6).WithMessage("Invalid card code");
        }
    }
}