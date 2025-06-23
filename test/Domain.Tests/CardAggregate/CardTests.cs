using Domain.Aggregates.CardAggregate;
using Domain.Aggregates.ProductAggregate;
using Domain.Aggregates.PurchaseAggregate;
using Domain.Shared.ValueObjects;
using FluentResults;

namespace Domain.Tests.CardAggregate;

public class CardTests
{
    private readonly CardNumber _number = CardNumber.Create("123456789012").Value;
    private readonly CardPinHash _pinHash = CardPinHash.Create("1234").Value;
    private readonly Quantity _balance = Quantity.Create(10m).Value;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Category _category = Category.Fuel;
    private readonly Quantity _one = Quantity.Create(1m).Value;
    private readonly string _productTitle = "Product";
    private readonly string _productDescription = "Description";
    private readonly decimal _productPrice = 1m; 
    private readonly Quantity  _productQuantity = Quantity.Create(100m).Value;
    
    [Fact]
    public void Create_SuccessCase_ReturnsSuccess()
    {
        var result = Card.Create(_number, _pinHash, _balance);
        
        var card = result.ValueOrDefault;
        Assert.True(result.IsSuccess);
        Assert.Equal(_number, card.Number);
        Assert.Equal(_pinHash, card.PinHash);
        Assert.Equal(_balance, card.Balance);
    }
    
    [Fact]
    public void Create_CardNumberIsNull_ReturnsFail()
    {
        var result = Card.Create(null, _pinHash, _balance);
        
        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public void Create_CardPinHashIsNull_ReturnsFail()
    {
        var result = Card.Create(_number, null, _balance);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Activate_UnUsedCard_ReturnsSuccess()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;

        var result = card.Activate(_userId);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(Status.Activated, card.Status);
        Assert.NotNull(card.ActivatedAt);
        Assert.True(card.ActivatedAt < DateTime.UtcNow);
    }

    [Fact]
    public void Activate_UserIdIsEmpty_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;

        var result = card.Activate(Guid.Empty);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Activate_ActivateFrozenCard_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        card.Freeze();

        var result = card.Activate(_userId);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Activate_ActivateAlreadyActivatedCard_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);

        var result = card.Activate(_userId);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Activate_ActivateBlockedCard_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        card.Block();

        var result = card.Activate(_userId);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Block_BlockActivatedCard_ReturnsSuccess()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        
        var result = card.Block();

        Assert.True(result.IsSuccess);
        Assert.Equal(Status.Blocked, card.Status);
    }
    
    [Fact]
    public void Block_BlockFrozenCard_ReturnsSuccess()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        card.Freeze();
        
        var result = card.Block();

        Assert.True(result.IsSuccess);
        Assert.Equal(Status.Blocked, card.Status);
    }

    [Fact]
    public void Block_BlockAlreadyBlockedCard_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        card.Block();

        var result = card.Block();
        
        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public void Block_BlockUnusedCard_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;

        var result = card.Block();

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Freeze_FreezeActivatedCard_ReturnsSuccess()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);

        var result = card.Freeze();
        
        Assert.True(result.IsSuccess);
        Assert.Equal(Status.Frozen, card.Status);
    }

    [Fact]
    public void Freeze_FreezeUnUsedCard_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;

        var result = card.Freeze();
        
        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public void Freeze_FreezeBlockedCard_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        card.Block();
        
        var result = card.Freeze();
        
        Assert.True(result.IsFailed);
    }
    
        
    [Fact]
    public void Freeze_FreezeAlreadyFrozenCard_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        card.Freeze();
        
        var result = card.Freeze();
        
        Assert.True(result.IsFailed);
    }
    
        
    [Fact]
    public void UnFreeze_UnFreezeFrozenCard_ReturnsSuccess()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        card.Freeze();
        
        var result = card.UnFreeze();
        
        Assert.True(result.IsSuccess);
        Assert.Equal(Status.Activated, card.Status);
    }
    
    [Fact]
    public void UnFreeze_UnFreezeUnUsedCard_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        
        var result = card.UnFreeze();
        
        Assert.True(result.IsFailed);
    }
        
    [Fact]
    public void UnFreeze_UnFreezeActivatedCard_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        
        var result = card.UnFreeze();
        
        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public void UnFreeze_UnFreezeBlockedCard_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        card.Block();
        
        var result = card.UnFreeze();
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void VerifyPin_SuccessCase_ReturnsSuccess()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;

        var result = card.VerifyPin(_pinHash);
        
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public void VerifyPin_WrongPinHash_ReturnsSuccess()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        var pinHashResult = CardPinHash.Create("0000");
        
        var result = card.VerifyPin(pinHashResult.Value);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Buy_SuccessCase_ReturnsSuccess()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        var purchase = Purchase.Create(card.Id).Value;
        var product = Product.Create(_productTitle, _productDescription, _productPrice, _one, _category).Value;
        purchase.AddOrUpdate(product, _one);
        var expectedBalance = Quantity.Create(9m).Value;

        var result = card.Buy(purchase);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedBalance, card.Balance);
        Assert.Contains(purchase, card.Purchases);
        Assert.Equal(Quantity.Create(0).Value, product.Quantity);
    }

    [Fact]
    public void Buy_PurchaseIsNull_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        var result = card.Buy(null);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Buy_CardHaveUnusedStatus_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        var purchase = Purchase.Create(card.Id).Value;

        var result = card.Buy(purchase);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Buy_CardHaveBlockedStatus_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        card.Block();
        var purchase = Purchase.Create(card.Id).Value;

        var result = card.Buy(purchase);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Buy_NoItemsInPurchase_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        var purchase = Purchase.Create(card.Id).Value;

        var result = card.Buy(purchase);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Buy_PurchaseTotalLitersGreaterThenCardBalance_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        var product = Product.Create(_productTitle, _productDescription, _productPrice, _productQuantity, _category).Value;
        var purchase = Purchase.Create(card.Id).Value;
        purchase.AddOrUpdate(product, _productQuantity);

        var result = card.Buy(purchase);

        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public void Buy_DuplicatePurchase_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        var purchase = Purchase.Create(card.Id).Value;
        var product = Product.Create(_productTitle, _productDescription, _productPrice, _one, _category).Value;
        purchase.AddOrUpdate(product, _one);
        card.Buy(purchase);

        var result = card.Buy(purchase);
        
        Assert.True(result.IsFailed);
    }

    [Fact]
    public void ReturnLatest_SuccessCase_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        var purchase = Purchase.Create(card.Id).Value;
        var product = Product.Create(_productTitle, _productDescription, _productPrice, _one, _category).Value;
        purchase.AddOrUpdate(product, _one);
        card.Buy(purchase);

        var result = card.ReturnLatest(product.Id, _productPrice, _one);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(_balance, card.Balance);
        Assert.NotEmpty(card.Refunds);
        Assert.Equal(_one, product.Quantity);
    }
    
    [Fact]
    public void ReturnLatest_PurchaseNotFoundForRefund_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;

        var result = card.ReturnLatest(1, 1m, _one);
        
        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public void ReturnLatest_SpecifiedQuantityGreaterThanAvailableQuantityForReturn_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        var purchase = Purchase.Create(card.Id).Value;
        var product = Product.Create(_productTitle, _productDescription, _productPrice, _one, _category).Value;
        purchase.AddOrUpdate(product, _one);
        card.Buy(purchase);
        card.ReturnLatest(product.Id, _productPrice, _one);
        
        var result = card.ReturnLatest(product.Id, _productPrice, _one);

        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public void ReturnLatest_ProductPriceAtRefundNotMatchProductPriceAtPurchase_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        var purchase = Purchase.Create(card.Id).Value;
        var product = Product.Create(_productTitle, _productDescription, 1m, _one, _category).Value;
        purchase.AddOrUpdate(product, _one);
        card.Buy(purchase);
        
        var result = card.ReturnLatest(product.Id, 2m, _one);

        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public void ReturnLatest_ProductQuantityAtRefundNotMatchProductQuantityAtPurchase_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        var purchase = Purchase.Create(card.Id).Value;
        var product = Product.Create(_productTitle, _productDescription, _productPrice, Quantity.Create(1m).Value, _category).Value;
        purchase.AddOrUpdate(product, _one);
        card.Buy(purchase);
        
        var result = card.ReturnLatest(product.Id, _productPrice, Quantity.Create(2m).Value);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void ReturnLatest_RepeatReturn_ReturnsSuccess()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        var purchase = Purchase.Create(card.Id).Value;
        var ten = Quantity.Create(10).Value;
        var product = Product.Create(_productTitle, _productDescription, _productPrice, ten, _category).Value;
        purchase.AddOrUpdate(product, ten);
        
        var repeatCount = 5;
        Result<Refund> result = default;
        
        for (var i = 0; i < repeatCount; i++)
        {
            card.Buy(purchase);
        }

        for (var i = 0; i < repeatCount; i++)
        {
            result = card.ReturnLatest(product.Id, _productPrice, _one);
        }
        
        Assert.True(result!.IsSuccess);
        Assert.Equal(_productPrice * repeatCount, card.Balance.Value);
        Assert.NotEmpty(card.Refunds);
        Assert.Equal(Quantity.Create(repeatCount).Value, product.Quantity);
    }

    [Fact]
    public void ChangePin_SuccessCase_ReturnsSuccess()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        var newPinHash = CardPinHash.Create("1234").Value;

        var result = card.ChangePin(newPinHash);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(newPinHash, card.PinHash);
    }

    [Fact]
    public void ChangePin_CannotChangePinForUnusedCard_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        var newPinHash = CardPinHash.Create("1234").Value;

        var result = card.ChangePin(newPinHash);
        
        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public void ChangePin_CannotChangePinForBlockedCard_ReturnsFail()
    {
        var card = Card.Create(_number, _pinHash, _balance).Value;
        card.Activate(_userId);
        card.Block();
        var newPinHash = CardPinHash.Create("1234").Value;

        var result = card.ChangePin(newPinHash);
        
        Assert.True(result.IsFailed);
    }
}