using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using MediatR;

namespace Application.Cards.Commands.Deactivate
{
    public record DeactivateCardCommand(int CardNumber) : IRequest<Result>;
}