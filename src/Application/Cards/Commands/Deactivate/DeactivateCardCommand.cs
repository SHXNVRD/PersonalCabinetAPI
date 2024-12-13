using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using MediatR;

namespace Application.Cards.Commands.Deactivate
{
    public class DeactivateCardCommand : IRequest<Result>
    {
        public int CardNumber { get; set; }
    }
}