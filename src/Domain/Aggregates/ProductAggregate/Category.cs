using Domain.Aggregates.Base;

namespace Domain.Aggregates.ProductAggregate
{
    public sealed class Category : Identity<Guid>
    {
        public string Title { get; private set; } = null!;
    }
}