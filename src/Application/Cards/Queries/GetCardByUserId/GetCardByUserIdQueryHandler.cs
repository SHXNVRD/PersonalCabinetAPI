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
    public class GetCardByUserIdQueryHandler : IRequestHandler<GetCardByUserIdQuery, Result<CardResponse>>
    {
        private readonly ICardRepository _cardRepository;

        public GetCardByUserIdQueryHandler(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public async Task<Result<CardResponse>> Handle(GetCardByUserIdQuery request, CancellationToken cancellationToken)
        {
            if (!long.TryParse(request.Id, out long userId))
                return Result.Fail("Failed to get card information");

            var card = await _cardRepository.FindByUserIdAsync(userId);

            if (card == null)
                return Result.Fail($"Card not found for current user");

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