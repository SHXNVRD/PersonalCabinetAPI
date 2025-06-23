using Domain.Aggregates.Base;
using Domain.Aggregates.ProductAggregate;
using Domain.Shared.Errors;
using Domain.Shared.ValueObjects;
using FluentResults;

namespace Domain.Aggregates.PurchaseAggregate;

public sealed class Purchase : Aggregate<Guid>
{
    public Guid CardId { get; private set; }
    public Check Check { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    private List<PurchaseItem> _purchaseItems = [];
    public IReadOnlyList<PurchaseItem> PurchaseItems => _purchaseItems.AsReadOnly();
    public decimal Total => PurchaseItems.Sum(pi => pi.Total);

    private Purchase()
    { }

    private Purchase(
        Guid cardId,
        Check check) : this()
    {
        Id = Guid.NewGuid();
        CardId = cardId;
        Check = check;
    }

    public static Result<Purchase> Create(Guid cardId)
    {
        if (cardId == Guid.Empty)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(cardId)} cannot be empty"));

        var checkResult = Check.Create();
        if (checkResult.IsFailed)
            return Result.Fail(checkResult.Errors);
        
        return  new Purchase(cardId, checkResult.Value);
    }

    public Result AddOrUpdate(Product product, Quantity quantity)
    {
        if (product == null)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(product)} cannot be null"));
        if (quantity > product.Quantity)
            return Result.Fail(Errors.Conflict.LowAvailableQuantity($"{nameof(quantity)} must be less than available product quantity"));
            
        var item = _purchaseItems.SingleOrDefault(pi => pi.Product.Id == product.Id);
        
        if (item != null)
        {
            var result = item.Add(quantity);
            if (result.IsFailed)
                return Result.Fail(result.Errors);
                
            return Result.Ok();
        }
        
        var newItemResult = PurchaseItem.Create(product, quantity);
        if (newItemResult.IsFailed)
            return Result.Fail(newItemResult.Errors);

        _purchaseItems.Add(newItemResult.Value);
            
        return Result.Ok();
    }

    public Result Remove(Product product, Quantity quantity)
    {
        var item = _purchaseItems.FirstOrDefault(pi => pi.Product.Id == product.Id);
        if (item == null)
            return Result.Fail(Errors.Conflict.NotFound("Product not found in purchase"));
                    
        var newQuantityResult = item.Remove(quantity);
        if (newQuantityResult.IsFailed)
            return Result.Fail(Errors.Conflict.LowAvailableQuantity("Cannot remove more quantity than exists"));

        if (item.Quantity.Value == 0)
            _purchaseItems.Remove(item);

        return Result.Ok();
    }
}