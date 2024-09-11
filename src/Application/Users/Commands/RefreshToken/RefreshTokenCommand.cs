using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using FluentResults;
using MediatR;

namespace Application.Users.Commands.RefreshToken
{
    public record RefreshTokenCommand(
        string AccessToken,
        string RefreshToken) : IRequest<Result<AuthResponse>>; 
}