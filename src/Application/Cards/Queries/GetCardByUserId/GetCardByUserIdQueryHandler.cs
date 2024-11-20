using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Application.Cards.Queries.GetCardByUserId
{
    public class GetCardByUserIdQueryHandler : IRequestHandler<GetCardByUserIdQuery, Result<CardResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCardByUserIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CardResponse>> Handle(GetCardByUserIdQuery request, CancellationToken cancellationToken)
        {
            var card = await _unitOfWork.CardRepository.FindByUserIdAsync(request.UserId);

            if (card == null)
                return Result.Fail($"For user with Id {request.UserId} card not found");

            return Result.Ok(new CardResponse
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