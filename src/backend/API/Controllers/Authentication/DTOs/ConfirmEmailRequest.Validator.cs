using API.Helpers;
using FluentValidation;

namespace API.Controllers.Authentication.DTOs;

public class ConfirmEmailRequestValidator : AbstractValidator<ConfirmEmailRequest>
{
    public ConfirmEmailRequestValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(r => r.Token)
            .NotEmpty().WithMessage("Token is required");

        RuleFor(r => r.RedirectUrl)
            .Must(StringHelper.IsUrl!).When(r => !string.IsNullOrWhiteSpace(r.RedirectUrl)).WithMessage("Invalid redirect url");
    }
}