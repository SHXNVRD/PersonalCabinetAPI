using FluentValidation;

namespace Application.Users.Commands.RevokeRefreshToken
{
    public class RevokeRefreshTokenCommandValidator : AbstractValidator<RevokeRefreshTokenCommand>
    {
        public RevokeRefreshTokenCommandValidator()
        {
            RuleFor(q => q.UserId).NotEmpty().WithMessage("Id is required");
        }
    }
}