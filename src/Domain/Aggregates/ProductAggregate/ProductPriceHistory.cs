using Domain.Aggregates.Base;

namespace Domain.Aggregates.ProductAggregate;

public sealed class ProductPriceHistory : Identity<Guid>
{
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }
}