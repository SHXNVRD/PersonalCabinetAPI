using FluentValidation;
using Microsoft.IdentityModel.Tokens;
using Quartz.Util;

namespace API.Controllers.Authentication.DTOs;

public class CreateEmailConfirmationLinkRequestValidator : AbstractValidator<CreateEmailConfirmationLinkRequest>
{
    public CreateEmailConfirmationLinkRequestValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("Invalid email address");

        RuleFor(r => r.RedirectUrl).Must(ValidateUrl).WithMessage("Invalid redirect url");
    }

    private bool ValidateUrl(string? url)
    {
        if (url.IsNullOrWhiteSpace())
            return true;

        return Uri.IsWellFormedUriString(url, UriKind.Absolute);
    }
}