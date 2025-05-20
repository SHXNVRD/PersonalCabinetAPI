using Domain.Aggregates.Base;
using Domain.Shared.Errors;
using FluentResults;

namespace Domain.Aggregates.ProductAggregate;

public sealed class ProductPriceHistory : Identity<Guid>
{
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    private ProductPriceHistory()
    { }

    private ProductPriceHistory(decimal price) : this()
        => Price = price;

    public static Result<ProductPriceHistory> Create(decimal price)
    {
        if (price <= 0)
            return Result.Fail(new InvalidData($"{nameof(price)} must be greater than zero"));

        return new ProductPriceHistory(price);
    }
}