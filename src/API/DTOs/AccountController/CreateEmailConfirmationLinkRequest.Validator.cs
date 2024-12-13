using Application.Users.Commands.CreateEmailConfirmationLink;
using FluentValidation;

namespace API.DTOs.AccountController;

public class CreateEmailConfirmationLinkRequestValidator : AbstractValidator<CreateEmailConfirmationLinkRequest>
{
    public CreateEmailConfirmationLinkRequestValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("Invalid email address");
    }
}