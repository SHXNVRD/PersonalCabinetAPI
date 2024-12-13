using Application.Users.Commands.RefreshToken;
using FluentValidation;

namespace API.DTOs.AccountController
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(r => r.RefreshToken).NotEmpty().WithMessage("Refresh token is required");
        }
    }
}