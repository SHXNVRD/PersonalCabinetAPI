using System.Reflection;
using Domain.Aggregates.Base;
using Domain.Aggregates.PurchaseAggregate;
using Domain.Shared.Errors;
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
            return Result.Fail(new InvalidData($"{nameof(number)} cannot be null"));
        if (pin is null)
            return Result.Fail(new InvalidData($"{nameof(pin)} cannot be null"));
        if (balance < 0)
            return Result.Fail(new InvalidData($"{nameof(balance)} must be greater or equals zero"));
            
        return new Card(number, pin, balance);
    }

    public Result Activate(Guid userId)
    {
        if (userId == Guid.Empty)
            return Result.Fail(new InvalidData($"{nameof(userId)} cannot be empty"));
        if (!Status.CanChangeTo(Status.Activated))
            return Result.Fail(new Conflict("Activation failed. Status cannot be settled"));

        UserId = userId;
        Status = Status.Activated;
        ActivatedAt = DateTime.UtcNow;
            
        return Result.Ok();
    }
    
    public Result Block()
    {
        if (!Status.CanChangeTo(Status.Blocked))
            return Result.Fail(new Conflict("Deactivation failed. Status cannot be settled"));

        Status = Status.Blocked;
            
        return Result.Ok();
    }

    public Result VerifyPin(CardPinHash pinHash)
        => Result.OkIf(pinHash == PinHash, new Conflict("Wrong card pin"));

    public Result Buy(Purchase purchase)
    {
        if (purchase is null)
            return Result.Fail(new InvalidData($"{nameof(purchase)} cannot be null"));
        if (Status != Status.Activated)
            return Result.Fail(new Conflict("Card must be activated to make purchases"));
        if (purchase.PurchaseItems.Count == 0)
            return Result.Fail(new Conflict("Purchase must contain at least one item"));
        if (Balance < purchase.Total)
            return Result.Fail(new LowCardBalance("Insufficient funds"));
        if (_purchases.Any(p => p.Id == purchase.Id))
            return Result.Fail(new Conflict("Duplicated purchase"));

        Balance -= purchase.Total;
        _purchases.Add(purchase);

        return Result.Ok();
    }
    
    public Result<Refund> RefundLatest(long productId, decimal productPrice, Quantity productQuantity)
    {
        var purchase = _purchases.MaxBy(p => p.CreatedAt);

        if (purchase == null)
            return Result.Fail(new Conflict("No purchases for refund"));
        
        if (!CanRefund(purchase, productId, productPrice, productQuantity))
            return Result.Fail(new Conflict("Cannot refund purchase"));

        var refundResult = Refund.Create(Id, purchase);
        if (refundResult.IsFailed)
            return Result.Fail(refundResult.Errors);

        var refund = refundResult.Value;
        
        var purchaseItem = purchase.PurchaseItems.Single(pi => pi.Product.Id == productId);
        purchaseItem.Product.Add(purchaseItem.Quantity);
        
        _refunds.Add(refund);
        Balance += refund.Total;
            
        return Result.Ok(refund);
    }
        
    private bool CanRefund(Purchase purchase, long productId, decimal productPrice, Quantity productQuantity)
    {
        if (_refunds.Any(r => r.PurchaseId == purchase.Id))
            return false;
        
        var purchaseItem = purchase.PurchaseItems.SingleOrDefault(pi => pi.Product.Id == productId);
        if (purchaseItem?.ProductPriceAtPurchase != productPrice || purchaseItem.Quantity != productQuantity)
            return false;

        return true;
    }
}