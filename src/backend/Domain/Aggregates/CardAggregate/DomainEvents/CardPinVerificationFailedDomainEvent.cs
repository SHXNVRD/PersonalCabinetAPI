using Domain.Shared;

namespace Domain.Aggregates.CardAggregate.DomainEvents;

public record CardPinVerificationFailedDomainEvent : DomainEvent
{
    public Guid CardId { get; }

    public CardPinVerificationFailedDomainEvent(Guid cardId)
    {
        if (cardId == Guid.Empty)
            throw new ArgumentException($"{nameof(cardId)} cannot be empty");

        CardId = cardId;
    }
}