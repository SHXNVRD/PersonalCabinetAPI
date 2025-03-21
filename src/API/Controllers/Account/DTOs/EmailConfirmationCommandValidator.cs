using Application.Users.Commands.EmailConfirmation;
using FluentValidation;

namespace API.Controllers.Account.DTOs;

public class EmailConfirmationCommandValidator : AbstractValidator<EmailConfirmationCommand>
{
    public EmailConfirmationCommandValidator()
    {
            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email address is required")
                .EmailAddress().WithMessage("Invalid email address");
        }
}