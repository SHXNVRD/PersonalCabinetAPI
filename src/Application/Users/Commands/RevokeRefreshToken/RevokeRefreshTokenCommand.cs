using FluentResults;
using MediatR;

namespace Application.Users.Commands.RevokeRefreshToken
{
    public record RevokeRefreshTokenCommand(string UserId) : IRequest<Result>;
}