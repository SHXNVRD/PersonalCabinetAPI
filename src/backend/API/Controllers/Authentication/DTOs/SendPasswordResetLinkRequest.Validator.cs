using FluentValidation;

namespace API.Controllers.Authentication.DTOs;

public class SendPasswordResetLinkRequestValidator : AbstractValidator<SendPasswordResetLinkRequest>
{
    public SendPasswordResetLinkRequestValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("Invalid email address");
    }
}