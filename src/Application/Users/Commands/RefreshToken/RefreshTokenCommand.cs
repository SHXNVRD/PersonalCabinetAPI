using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Users.DTOs;
using FluentResults;
using MediatR;

namespace Application.Users.Commands.RefreshToken
{
    public record RefreshTokenCommand(
        string UserId,
        string RefreshToken) : IRequest<Result<AuthResponse>>; 
}