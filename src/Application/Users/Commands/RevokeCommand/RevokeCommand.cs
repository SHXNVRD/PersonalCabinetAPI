using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using FluentResults;
using MediatR;

namespace Application.Users.Commands
{
    public record RevokeCommand(string UserEmail) : IRequest<Result>;
}