using API.Helpers;
using FluentValidation;

namespace API.Controllers.Authentication.DTOs;

public class CreateEmailConfirmationLinkRequestValidator : AbstractValidator<CreateEmailConfirmationLinkRequest>
{
    public CreateEmailConfirmationLinkRequestValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(r => r.RedirectUrl)
            .Must(StringHelper.IsUrl).When(r => !string.IsNullOrWhiteSpace(r.RedirectUrl)).WithMessage("Invalid redirect url");
    }
}