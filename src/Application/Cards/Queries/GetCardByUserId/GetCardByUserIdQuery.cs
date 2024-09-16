using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using FluentResults;
using MediatR;

namespace Application.Cards.Queries
{
    public record GetCardByUserIdQuery(string Id) : IRequest<Result<CardResponse>>;
}