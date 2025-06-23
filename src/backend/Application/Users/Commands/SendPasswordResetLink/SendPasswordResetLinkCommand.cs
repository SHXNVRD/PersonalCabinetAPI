using FluentResults;
using MediatR;

namespace Application.Users.Commands.SendPasswordResetLink;

public class SendPasswordResetLinkCommand : IRequest<Result>
{
    public string Email { get; set; }
}