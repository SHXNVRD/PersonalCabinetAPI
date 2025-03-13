using Domain.Aggregates.Base;
using FluentResults;

namespace Domain.Aggregates.PurchaseAggregate;

public sealed class Refund : Identity<Guid>
{
    public Guid CardId { get; private set; }
    public Guid PurchaseId { get; private set; }
    public Check Check { get; private set; } = null!;
    public decimal Total { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Refund()
    { }

    private Refund(
        Guid cardId,
        Guid purchaseId,
        Check check,
        decimal total,
        DateTime createdAt) : this()
    {
        CardId = cardId;
        PurchaseId = purchaseId;
        Check = check;
        Total = total;
        CreatedAt = createdAt;
    }

    public static Result<Refund> Create(Guid cardId, Purchase purchase)
    {
        if (purchase == null)
            return Result.Fail($"{nameof(purchase)} cannot be null");
        if (cardId == Guid.Empty)
            return Result.Fail($"{nameof(cardId)} cannot be empty");

        var checkResult = Check.Create();
        if (checkResult.IsFailed)
            return Result.Fail(checkResult.Errors);

        return new Refund(cardId, purchase.Id, checkResult.Value, purchase.Total, DateTime.UtcNow);
    }
}