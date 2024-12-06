using FluentResults;
using MediatR;

namespace Application.Users.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Email,
    string Token,
    string NewPassword) : IRequest<Result>;