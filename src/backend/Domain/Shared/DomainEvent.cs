using MediatR;

namespace Domain.Shared;

public record DomainEvent : INotification
{
    public Guid Id { get; private set; } = Guid.NewGuid();
}