using FluentValidation;
using Org.BouncyCastle.Math.EC.Multiplier;

namespace API.DTOs.AccountController;

public class SendPasswordResetLinkRequestValidator : AbstractValidator<SendPasswordResetLinkRequest>
{
    public SendPasswordResetLinkRequestValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("Email address is required")
            .EmailAddress().WithMessage("Invalid email address");
    }
}