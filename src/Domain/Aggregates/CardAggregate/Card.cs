using System.Net.NetworkInformation;
using Domain.Aggregates.Base;
using Domain.Aggregates.PurchaseAggregate;
using Domain.Shared.ValueObjects;
using FluentResults;

namespace Domain.Aggregates.CardAggregate;

public sealed class Card : Aggregate<Guid>
{
    public Guid? UserId { get; private set; }
    public CardNumber Number { get; private set; } = null!;
    public CardPinHash PinHash { get; private set; } = null!;
    public decimal Balance { get; private set; }
    public DateTime? ActivatedAt { get; private set; }
    public Status Status { get; private set; } = null!;
        
    private List<Purchase> _purchases = [];
    public IReadOnlyList<Purchase> Purchases => _purchases.AsReadOnly();
        
    private List<Refund> _refunds = [];
    public IReadOnlyList<Refund> Refunds => _refunds.AsReadOnly();
        
    private Card()
    { }

    private Card(
        CardNumber number,
        CardPinHash pinHash,
        decimal balance) : this()
    {
        Id = Guid.NewGuid();
        Number = number;
        PinHash = pinHash;
        Balance = balance;
        Status = Status.Unused;
    }

    public static Result<Card> Create(CardNumber number, CardPinHash pin, decimal balance)
    {
        if (number is null)
            return Result.Fail($"{nameof(number)} cannot be null");
        if (pin is null)
            return Result.Fail($"{nameof(pin)} cannot be null");
        if (balance < 0)
            return Result.Fail($"{nameof(balance)} must be greater or equals zero");
            
        return new Card(number, pin, balance);
    }

    internal Result Activate(Guid userId)
    {
        if (userId == Guid.Empty)
            return Result.Fail($"{nameof(userId)} cannot be empty");
        if (Status.CanChangeTo(Status.Activated))
            return Result.Fail("Status cannot be settled");

        UserId = userId;
        Status = Status.Activated;
        ActivatedAt = DateTime.UtcNow;
            
        return Result.Ok();
    }

    public Result Buy(Purchase purchase)
    {
        if (Status != Status.Activated)
            return Result.Fail("Card must be activated to make purchases");
        if (purchase is null)
            return Result.Fail($"{nameof(purchase)} cannot be null");
        if (Balance < purchase.Total)
            return Result.Fail("Insufficient funds");
            
        if (_purchases.Any(p => p.Id == purchase.Id))
            return Result.Fail("Duplicated purchase");

        Balance -= purchase.Total;
        _purchases.Add(purchase);

        return Result.Ok();
    }

    public Result RefundLatest()
    {
        var refundingPurchase = _purchases
            .MaxBy(p => p.CreatedAt);

        if (refundingPurchase == null)
            return Result.Fail("No purchases for refund");

        return RefundPurchase(refundingPurchase);
    }

    private Result RefundPurchase(Purchase purchase)
    {
        if (purchase == null)
            return Result.Fail($"{nameof(purchase)} cannot be null");

        var canRefundResult = CanRefund(purchase);
        if (canRefundResult.IsFailed)
            return Result.Fail(canRefundResult.Errors);

        var refundResult = Refund.Create(Id, purchase);
        if (refundResult.IsFailed)
            return Result.Fail(refundResult.Errors);

        var refund = refundResult.Value;
        _refunds.Add(refund);
        Balance += refund.Total;
            
        return Result.Ok();
    }
        
    private Result CanRefund(Purchase purchase)
    {
        var refund = _refunds.SingleOrDefault(r => r.PurchaseId == purchase.Id);

        if (refund != null)
            return Result.Fail($"{nameof(purchase)} already refunded");

        return Result.Ok();
    }
}