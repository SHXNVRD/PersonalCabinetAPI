using Domain.Aggregates.Base;
using Domain.Aggregates.ProductAggregate;
using Domain.Shared.Errors;
using Domain.Shared.ValueObjects;
using FluentResults;

namespace Domain.Aggregates.PurchaseAggregate;

public sealed class PurchaseItem : Identity<Guid>
{
    public Product Product { get; private set; } = null!;
    public Quantity Quantity { get; private set; } = null!;
    public decimal ProductPriceAtPurchase { get; private set; }
    public decimal Total => (decimal)Quantity.Value * ProductPriceAtPurchase;

    private PurchaseItem()
    { }

    private PurchaseItem(
        Product product,
        Quantity quantity,
        decimal productPriceAtPurchase) : this()
    {
        Id = Guid.NewGuid();
        Product = product;
        Quantity = quantity;
        ProductPriceAtPurchase = productPriceAtPurchase;
    }

    public static Result<PurchaseItem> Create(Product product, Quantity quantity)
    {
        if (product == null)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(product)} cannot be null"));

        var removeResult = product.Remove(quantity);
        if (removeResult.IsFailed)
            return Result.Fail(
                Errors.Conflict.LowAvailableQuantity($"{nameof(quantity)} must be less than the available product quantity")
                    .CausedBy(removeResult.Errors));
        
        return new PurchaseItem(product, quantity, product.Price);
    }

    public Result Add(Quantity quantity)
    {  
        var addResult = Quantity.Add(quantity);
        if (addResult.IsFailed)
            return Result.Fail(addResult.Errors);

        var removeProductQuantityResult = Product.Remove(quantity);
        if (removeProductQuantityResult.IsFailed)
            return Result.Fail(
                Errors.Conflict.LowAvailableQuantity($"{nameof(quantity)} must be less than the available product quantity")
                    .CausedBy(removeProductQuantityResult.Errors));
                
        Quantity = addResult.Value;
        
        return Result.Ok();
    }

    public Result Remove(Quantity quantity)
    {
        var updateResult = Quantity.Subtract(quantity);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors);
        
        var addProductQuantityResult = Product.Add(quantity);
        if (addProductQuantityResult.IsFailed)
            return Result.Fail(addProductQuantityResult.Errors);

        Quantity = updateResult.Value;
            
        return Result.Ok();
    }
}