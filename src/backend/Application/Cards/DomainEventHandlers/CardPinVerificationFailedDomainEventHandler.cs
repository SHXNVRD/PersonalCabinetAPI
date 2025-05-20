using Application.Interfaces;
using Domain.Aggregates.CardAggregate.DomainEvents;
using Domain.Shared.Exceptions;
using MediatR;

namespace Application.Cards.DomainEventHandlers;

public class CardPinVerificationFailedDomainEventHandler : INotificationHandler<CardPinVerificationFailedDomainEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public CardPinVerificationFailedDomainEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CardPinVerificationFailedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var card = await _unitOfWork.CardRepository.FindByIdAsync(domainEvent.CardId);
        if (card is null)
            throw new DataConsistencyViolationException($"{nameof(CardPinVerificationFailedDomainEvent)} contains {nameof(domainEvent.CardId)} for non-exist card");

        card.IncrementVerificationAttempt();
        await _unitOfWork.SaveChangesAsync();
    }
}