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
    public class RefreshTokenCommand : IRequest<Result<AuthResponse>>
    {
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
    }
}