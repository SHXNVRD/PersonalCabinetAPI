using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using FluentResults;
using MediatR;

namespace Application.Users.Commands.Registration
{
    public record RegistrationCommand(
        string UserName,
        string PhoneNumber,
        string Email,
        string Password) : IRequest<Result<AuthResponse>>;
}