using FluentResults;
using MediatR;

namespace Application.Users.Commands.EmailConfirmation
{
    public record EmailConfirmationCommand(
        string Email,
        string Token) : IRequest<Result>;
}