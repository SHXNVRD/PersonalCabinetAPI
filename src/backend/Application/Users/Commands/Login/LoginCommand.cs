using Application.Users.DTOs;
using FluentResults;
using MediatR;

namespace Application.Users.Commands.Login;

public class LoginCommand : IRequest<Result<AuthResponse>>
{
    public string Password { get; set; } = null!;
    
    public string Login { get; set; } = null!;
}