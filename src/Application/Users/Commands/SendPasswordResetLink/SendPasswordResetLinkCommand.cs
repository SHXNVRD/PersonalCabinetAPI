using FluentResults;
using MediatR;

namespace Application.Users.Commands.SendPasswordResetLink;

public record SendPasswordResetLinkCommand(string Email) : IRequest<Result>;