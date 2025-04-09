using Domain.Shared;

namespace Domain.Aggregates.CardAggregate.DomainEvents;

public record CardPinVerificationAttemptsExceededDomainEvent : DomainEvent
{
    public Guid CardId { get; }

    public CardPinVerificationAttemptsExceededDomainEvent(Guid cardId)
    {
        if (cardId == Guid.Empty)
            throw new ArgumentException($"{nameof(cardId)} cannot be empty");

        CardId = cardId;
    }
}