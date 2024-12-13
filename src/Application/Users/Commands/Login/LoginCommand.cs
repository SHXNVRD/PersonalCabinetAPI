using Application.Users.DTOs;
using FluentResults;
using MediatR;

namespace Application.Users.Commands.Login
{
    public class LoginCommand : IRequest<Result<AuthResponse>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
