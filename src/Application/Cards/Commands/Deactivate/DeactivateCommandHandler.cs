using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.Repositories;
using FluentResults;
using MediatR;

namespace Application.Cards.Commands.Deactivate
{
    public class DeactivateCommandHandler : IRequestHandler<DeactivateCardCommand, Result>
    {
        private readonly ICardRepository _cardRepository;

        public DeactivateCommandHandler(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        public async Task<Result> Handle(DeactivateCardCommand request, CancellationToken cancellationToken)
        {
            var deactivateResult = await _cardRepository.DeactivateAsync(request.CardNumber);

            return Result.OkIf(deactivateResult, $"Failed to deactivate card with number: {request.CardNumber}");
        }
    }
}