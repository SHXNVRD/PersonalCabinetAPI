using Application.Users.DTOs;
using FluentResults;
using MediatR;

namespace Application.Users.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<Result<AuthResponse>>
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}