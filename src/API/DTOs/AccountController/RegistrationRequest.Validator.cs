using Application.Users.Commands.Registration;
using FluentValidation;

namespace API.DTOs.AccountController
{
    public class RegistrationRequestValidator : AbstractValidator<RegistrationRequest>
    {
        public RegistrationRequestValidator()
        {
            RuleFor(r => r.UserName)
                .NotEmpty().WithMessage("User name cannot be empty");

            RuleFor(r => r.Email)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Invalid email address");

            RuleFor(r => r.PhoneNumber)
                .NotEmpty().WithMessage("Phone number is required")
                .Length(10).WithMessage("Invalid phone number");

             RuleFor(r => r.Password)
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