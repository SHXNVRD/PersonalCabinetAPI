using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Application.Cards.Queries
{
    public class GetCardByIdQueryHandler : IRequestHandler<GetCardByIdQuery, Result<CardResponse>>
    {
        private readonly ICardRepository _cardRepository;

        public GetCardByIdQueryHandler(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public async Task<Result<CardResponse>> Handle(GetCardByIdQuery request, CancellationToken cancellationToken)
        {
            var card = await _cardRepository.FindByIdAsync(request.Id);

            if (card == null)
                return Result.Fail($"Card with id {request.Id} not found");

            return Result.Ok(new CardResponse()
            {
                Id = card.Id,
                Number = card.Number,
                IsActivated = card.IsActivated,
                BonusSystemTitle = card.BonusSystem.Title,
                DiscountPercent = card.BonusSystem.DiscountPercent
            });
        }
    }
}