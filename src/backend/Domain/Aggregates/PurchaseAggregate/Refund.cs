using Domain.Aggregates.Base;
using Domain.Shared.Errors;
using Domain.Shared.ValueObjects;
using FluentResults;

namespace Domain.Aggregates.PurchaseAggregate;

public sealed class Refund : Identity<Guid>
{
    public Guid CardId { get; private set; }
    public Guid PurchaseItemId { get; private set; }
    public Check Check { get; private set; } = null!;
    public Quantity Quantity { get; private set; }
    public decimal Total { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private Refund()
    { }

    private Refund(
        Guid cardId,
        Guid purchaseItemId,
        Check check,
        Quantity quantity,
        decimal total) : this()
    {
        Id = Guid.NewGuid();
        CardId = cardId;
        PurchaseItemId = purchaseItemId;
        Check = check;
        Quantity = quantity;
        Total = total;
    }

    public static Result<Refund> Create(Guid cardId, PurchaseItem purchaseItem, Quantity quantity)
    {
        if (quantity is null)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(quantity)} cannot be null"));
        if (purchaseItem == null)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(purchaseItem)} cannot be null"));
        if (cardId == Guid.Empty)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(cardId)} cannot be empty"));
        if (purchaseItem.Quantity < quantity)
            return Result.Fail(Errors.Conflict.LowAvailableQuantity("Refund product quantity exceeds the possible refund quantity"));
        
        var checkResult = Check.Create();
        if (checkResult.IsFailed)
            return Result.Fail(checkResult.Errors);

        var total = quantity.Value * purchaseItem.ProductPriceAtPurchase;
        
        purchaseItem.Product.Add(quantity);

        return new Refund(cardId, purchaseItem.Id, checkResult.Value, quantity, total);
    }
}