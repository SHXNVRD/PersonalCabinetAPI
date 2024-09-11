using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using MediatR;

namespace Application.Users.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(string Email) : IRequest;
}