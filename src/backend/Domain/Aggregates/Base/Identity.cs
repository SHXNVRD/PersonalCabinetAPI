namespace Domain.Aggregates.Base;

public abstract class Identity<TId> : IHaveId<TId>
{
    public TId Id { get; protected set; }
}