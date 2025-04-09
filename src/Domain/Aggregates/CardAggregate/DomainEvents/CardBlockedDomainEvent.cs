using Domain.Aggregates.UserAggregate;
using Domain.Shared;

namespace Domain.Aggregates.CardAggregate.DomainEvents;

public record CardBlockedDomainEvent : DomainEvent
{
    public Guid UserId { get; }
    public string CardNumber { get; }

    public CardBlockedDomainEvent(Guid userId, string cardNumber)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException($"{nameof(userId)} cannot be empty");
        if (string.IsNullOrWhiteSpace(cardNumber))
            throw new ArgumentException($"{nameof(cardNumber)} cannot be empty");

        UserId = userId;
        CardNumber = cardNumber;
    }
}