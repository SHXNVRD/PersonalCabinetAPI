namespace Domain.Aggregates.Base;

public interface IHaveId<out TId>
{
    TId Id { get; }
}