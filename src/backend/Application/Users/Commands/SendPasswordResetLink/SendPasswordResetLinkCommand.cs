using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Application.Users.Commands.SendPasswordResetLink;

public class SendPasswordResetLinkCommand : IRequest<Result>
{
    public string Email { get; set; }
}