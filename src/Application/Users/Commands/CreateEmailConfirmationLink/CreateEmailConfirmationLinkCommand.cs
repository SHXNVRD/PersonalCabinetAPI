using FluentResults;
using MediatR;

namespace Application.Users.Commands.CreateEmailConfirmationLink
{
    public record CreateEmailConfirmationLinkCommand(string Email) : IRequest<Result>;
}