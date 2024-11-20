using Application.DTOs;
using FluentResults;
using MediatR;

namespace Application.Cards.Queries.GetCardByUserId
{
    public record GetCardByUserIdQuery(long UserId) : IRequest<Result<CardResponse>>;
}