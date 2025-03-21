using FluentValidation;

namespace API.Controllers.Account.DTOs;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(r => r.RefreshToken).NotEmpty().WithMessage("Refresh token is required");
    }
}