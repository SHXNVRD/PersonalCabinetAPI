using Domain.Aggregates.Base;
using FluentResults;

namespace Domain.Aggregates.CardAggregate;

public sealed class Status : Identity<int>
{
    public static readonly Status Unused = new(1, nameof(Unused).ToLowerInvariant());
    public static readonly Status Activated = new(2, nameof(Activated).ToLowerInvariant());
    public static readonly Status Blocked = new(3, nameof(Blocked).ToLowerInvariant());
    public string Title { get; private set; } = null!;
    
    private Status()
    { }

    private Status(int id, string title) : this()
    {
        Id = id;
        Title = title;
    }

    public static IEnumerable<Status> All()
        => [Unused, Activated, Blocked];

    public bool CanChangeTo(Status status)
    {
        ArgumentNullException.ThrowIfNull(status);
        
        if (!All().Contains(status))
            throw new ArgumentOutOfRangeException(nameof(status), "Unsupported status");

        return status switch
        {
            _ when this == status => false,
            _ when this == Unused && status == Activated => true,
            _ when this == Activated && status == Blocked => true,
            _ => false
        };
    }
    
    public static bool operator ==(Status? a, Status? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Id == b.Id;
    }

    public static bool operator !=(Status a, Status b)
    {
        return !(a == b);
    }
}