using Domain.Aggregates.Base;
using Domain.Aggregates.ProductAggregate;
using Domain.Shared.ValueObjects;
using FluentResults;

namespace Domain.Aggregates.PurchaseAggregate;

public sealed class PurchaseItem : Identity<Guid>
{
    public long ProductId { get; private set; }
    public Quantity Quantity { get; private set; } = null!;
    public decimal ProductPriceAtPurchase { get; private set; }
    public decimal Total => (decimal)Quantity.Value * ProductPriceAtPurchase;

    private PurchaseItem()
    { }

    private PurchaseItem(
        long productId,
        Quantity quantity,
        decimal productPriceAtPurchase) : this()
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantity = quantity;
        ProductPriceAtPurchase = productPriceAtPurchase;
    }

    public static Result<PurchaseItem> Create(Product product, Quantity quantity)
    {
        if (product == null)
            return Result.Fail($"{nameof(product)} cannot be null");
        if (quantity > product.Quantity)
            return Result.Fail($"{nameof(quantity)} must be less than the available product quantity");

        return new PurchaseItem(product.Id, quantity, product.Price);
    }

    public Result Add(Quantity quantity)
    {  
        var updateResult = Quantity.Add(quantity);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors);

        Quantity = updateResult.Value;
            
        return Result.Ok();
    }

    public Result Remove(Quantity quantity)
    {
        var updateResult = Quantity.Subtract(quantity);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors);

        Quantity = updateResult.Value;
            
        return Result.Ok();
    }
}