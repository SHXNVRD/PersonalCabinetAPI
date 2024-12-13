using FluentResults;
using MediatR;

namespace Application.Users.Commands.RevokeRefreshToken
{
    public class RevokeRefreshTokenCommand : IRequest<Result>
    {
        public string UserId { get; set; }
    }
}