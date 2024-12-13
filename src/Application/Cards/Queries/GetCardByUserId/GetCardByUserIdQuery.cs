using Application.Cards.DTOs;
using Application.DTOs;
using FluentResults;
using MediatR;

namespace Application.Cards.Queries.GetCardByUserId
{
    public class GetCardByUserIdQuery : IRequest<Result<CardResponse>>
    {
        public long Id { get; set; }
    }
}