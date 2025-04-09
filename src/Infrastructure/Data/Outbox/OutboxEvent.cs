namespace Infrastructure.Data.Outbox;

public class OutboxEvent
{
    public required Guid EventId { get; init; }

    public required string Type { get; init; }

    public required string Content { get; init; }

    public required DateTime CreatedAt { get; init; }

    public DateTime? CompletedAt { get; private set; }


    public void MarkProcessed()
    {
        CompletedAt = DateTime.UtcNow;
    }
}