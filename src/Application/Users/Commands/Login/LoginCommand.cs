using Application.DTOs;
using FluentResults;
using MediatR;

namespace Application.Users.Commands.Login
{
    public record LoginCommand(
        string Email,
        string Password) : IRequest<Result<AuthResponse>>;
}
