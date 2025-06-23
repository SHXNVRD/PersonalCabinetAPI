using FluentValidation;

namespace API.Controllers.Authentication.DTOs;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(r => r.RefreshToken).NotEmpty().WithMessage("Refresh token is required");
        
        RuleFor(r => r.AccessToken).NotEmpty().WithMessage("Access token is required");
    }
}