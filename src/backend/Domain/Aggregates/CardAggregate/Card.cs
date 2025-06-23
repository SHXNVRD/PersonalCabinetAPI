using Domain.Aggregates.Base;
using Domain.Aggregates.CardAggregate.DomainEvents;
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
    // Литры
    public Quantity Balance { get; private set; }
    public DateTime? ActivatedAt { get; private set; }
    public Status Status { get; private set; } = null!;
    public int FailedVerifyAttempts { get; private set; }
        
    private List<Purchase> _purchases = [];
    public IReadOnlyList<Purchase> Purchases => _purchases.AsReadOnly();
        
    private List<Refund> _refunds = [];
    public IReadOnlyList<Refund> Refunds => _refunds.AsReadOnly();
        
    private Card()
    { }

    private Card(
        CardNumber number,
        CardPinHash pinHash,
        Quantity balance) : this()
    {
        Id = Guid.NewGuid();
        Number = number;
        PinHash = pinHash;
        Balance = balance;
        Status = Status.Unused;
    }

    public static Result<Card> Create(CardNumber number, CardPinHash pin, Quantity balance)
    {
        if (number is null)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"Card {nameof(number)} cannot be null"));
        if (pin is null)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"Card {nameof(pin)} cannot be empty"));
        if (balance is null)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"Card {nameof(balance)} cannot be"));
            
        return new Card(number, pin, balance);
    }

    public Result Activate(Guid userId)
    {
        if (userId == Guid.Empty)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(userId)} cannot be empty"));
        if (!Status.CanChangeTo(Status.Activated) || Status == Status.Frozen)
            return Result.Fail(Errors.Conflict.InvalidCurrentCardStatus(Status));

        UserId = userId;
        Status = Status.Activated;
        ActivatedAt = DateTime.UtcNow;
            
        return Result.Ok();
    }

    public Result Freeze()
    {
        if (!Status.CanChangeTo(Status.Frozen))
            return Result.Fail(Errors.Conflict.InvalidCurrentCardStatus(Status));

        Status = Status.Frozen;
        
        return Result.Ok();
    }

    public Result UnFreeze()
    {
        if (!Status.CanChangeTo(Status.Activated) || Status == Status.Unused)
            return Result.Fail(Errors.Conflict.InvalidCurrentCardStatus(Status));

        Status = Status.Activated;

        return Result.Ok();
    }
    
    public Result Block()
    {
        if (!Status.CanChangeTo(Status.Blocked))
            return Result.Fail(Errors.Conflict.InvalidCurrentCardStatus(Status));

        Status = Status.Blocked;
        AddDomainEvent(new CardBlockedDomainEvent(UserId!.Value, Number.Value));
            
        return Result.Ok();
    }

    public Result VerifyPin(CardPinHash pinHash)
    {
        if (PinHash == pinHash)
        {
            FailedVerifyAttempts = 0;
            return Result.Ok();
        }
        
        AddDomainEvent(new CardPinVerificationFailedDomainEvent(Id));
        
        return Result.Fail(Errors.Conflict.WrongCardPin());
    }
    
    public void IncrementVerificationAttempt()
    {
        if (++FailedVerifyAttempts >= 4)
            AddDomainEvent(new CardPinVerificationAttemptsExceededDomainEvent(Id));
    }

    public Result Buy(Purchase purchase)
    {
        var canBuyResult = CanBuy(purchase);
        if (canBuyResult.IsFailed)
            return Result.Fail(canBuyResult.Errors);

        var totalLiters = purchase.PurchaseItems.Sum(pi => pi.Quantity.Value);
        var totalLitersQuantity = Quantity.Create(totalLiters).Value;

        Balance = Balance.Subtract(totalLitersQuantity).Value;
        _purchases.Add(purchase);

        return Result.Ok();
    }

    private Result CanBuy(Purchase purchase)
    {
        if (purchase is null)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(purchase)} cannot be empty"));
        if (Status != Status.Activated)
            return Result.Fail(Errors.Conflict.InvalidCurrentCardStatus(Status, "Card must be activated to make purchases"));
        if (purchase.PurchaseItems.Count == 0)
            return Result.Fail(Errors.InvalidData.ValidationFailed("Purchase must contain at least one item"));
        
        var totalLiters = purchase.PurchaseItems.Sum(pi => pi.Quantity.Value);
        var totalLitersQuantity = Quantity.Create(totalLiters).Value;
        
        if (Balance < totalLitersQuantity)
            return Result.Fail(Errors.Conflict.InsufficientCardFunds(""));
        if (_purchases.Any(p => p.Id == purchase.Id))
            return Result.Fail(Errors.Conflict.Duplicate("Duplicated purchase"));

        return Result.Ok();
    }
    
    public Result<Refund> ReturnLatest(long productId, decimal productPrice, Quantity productQuantity)
    {
        var purchaseItemResult = GetPurchaseItemToRefund(productId, productPrice, productQuantity);
        if (purchaseItemResult.IsFailed)
            return Result.Fail(purchaseItemResult.Errors);

        var purchaseItem = purchaseItemResult.Value;
        
        var refundResult = Refund.Create(Id, purchaseItem, productQuantity);
        if (refundResult.IsFailed)
            return Result.Fail(refundResult.Errors);
        
        var refund = refundResult.Value;
        _refunds.Add(refund);
        
        Balance = Balance.Add(refund.Quantity).Value;
            
        return Result.Ok(refund);
    }

    private Result<PurchaseItem> GetPurchaseItemToRefund(long productId, decimal productPrice, Quantity productQuantity)
    {
        var purchase = _purchases.MaxBy(p => p.CreatedAt);
        if (purchase == null)
            return Result.Fail(Errors.Conflict.FailedReturnFunds("No purchases found on the card"));
        
        var purchaseItem = purchase.PurchaseItems.SingleOrDefault(pi => pi.Product.Id == productId);
        if (purchaseItem?.ProductPriceAtPurchase != productPrice || purchaseItem.Quantity < productQuantity)
            return Result.Fail(Errors.Conflict.FailedReturnFunds());

        var alreadyReturned = _refunds
            .Where(r => r.PurchaseItemId == purchaseItem.Id)
            .Sum(r => r.Quantity.Value);

        var potentialReturnedResult = Quantity.Create(alreadyReturned + productQuantity.Value);
        if (potentialReturnedResult.IsFailed)
            return Result.Fail(potentialReturnedResult.Errors);

        if (potentialReturnedResult.Value > purchaseItem.Quantity)
            return Result.Fail(Errors.Conflict.LowAvailableQuantity("Specified quantity greater than quantity available for return"));
        
        return purchaseItem;
    }

    public Result ChangePin(CardPinHash pin)
    {
        if (pin is null)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"Card {nameof(pin)} cannot be empty"));
        if (Status != Status.Activated)
            return Result.Fail(Errors.Conflict.InvalidCurrentCardStatus(Status, "Card must be activated to change pin"));

        PinHash = pin;

        return Result.Ok();
    }
}