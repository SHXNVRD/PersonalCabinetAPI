using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using FluentResults;
using MediatR;

namespace Application.Cards.Commands.Activate
{
    public record ActivateCardCommand(
        string UserId,
        int CardNumber,
        string CardCode) : IRequest<Result>;
}