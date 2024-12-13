namespace Domain.Models.Base;

public abstract class Identity : IHaveId
{
    public long Id { get; set; }
}