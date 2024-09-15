using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Application.Users.Commands.Login
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(c => c.Password)
                .NotEmpty().WithMessage("Your password cannot be empty")
                .MinimumLength(8).WithMessage("Your password length must be at least 8.")
                .MaximumLength(20).WithMessage("Your password length must not exceed 20.")
                .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
                .Matches(@"[\!\?\*\.\#\&\+\-\@]+").WithMessage("Your password must contain at least one (!?*.+-$#@).");
        }
    }
}