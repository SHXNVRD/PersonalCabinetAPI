using Application.Interfaces;
using Domain.Aggregates.CardAggregate.DomainEvents;
using Domain.Shared.Exceptions;
using MediatR;

namespace Application.Cards.DomainEventHandlers;

public class CardPinVerificationAttemptsExceededDomainEventHandler
    : INotificationHandler<CardPinVerificationAttemptsExceededDomainEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public CardPinVerificationAttemptsExceededDomainEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CardPinVerificationAttemptsExceededDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var card = await _unitOfWork.CardRepository.FindByIdAsync(domainEvent.CardId);
        if (card is null)
            throw new DataConsistencyViolationException($"{nameof(CardPinVerificationAttemptsExceededDomainEvent)} contains {nameof(domainEvent.CardId)} for non-exist card");

        var result = card.Block();
        if (result.IsFailed)
            throw new InvalidOperationException($"{result.Errors.First().Message} current status: {card.Status}");

        await _unitOfWork.SaveChangesAsync();
    }
}