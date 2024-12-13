using Application.Users.Commands.EmailConfirmation;
using FluentValidation;

namespace API.DTOs.AccountController
{
    public class EmailConfirmationCommandValidator : AbstractValidator<EmailConfirmationCommand>
    {
        public EmailConfirmationCommandValidator()
        {
            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Invalid email address");
        }
    }
}