using Application.Interfaces;
using Domain.Aggregates.PurchaseAggregate;
using Domain.Shared.Errors;
using Domain.Shared.ValueObjects;
using FluentResults;
using MediatR;

namespace Application.Purchases.Commands;

public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, Result<CreatePurchaseResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePurchaseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreatePurchaseResponse>> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var cardNumberResult = CardNumber.Create(request.CardNumber);
        if (cardNumberResult.IsFailed)
            return Result.Fail(cardNumberResult.Errors);
        
        var card = await _unitOfWork.CardRepository.FindByNumberAsync(cardNumberResult.Value, TrackingType.Tracking);

        if (card == null)
            return Result.Fail(Errors.Conflict.NotFound($"Card with number {request.CardNumber} was not found"));

        var cardPinHashResult = CardPinHash.Create(request.CardPin);
        if (cardPinHashResult.IsFailed)
            return Result.Fail(cardPinHashResult.Errors);

        var pinVerifyResult = card.VerifyPin(cardPinHashResult.Value);
        if (pinVerifyResult.IsFailed)
            return Result.Fail(pinVerifyResult.Errors);

        var product = await _unitOfWork.ProductRepository.FindById(request.ProductId, TrackingType.Tracking);

        if (product == null)
            return Result.Fail(Errors.Conflict.NotFound($"Product with id {request.ProductId} was not found"));
        if (product.Price != request.ProductPrice)
            return Result.Fail(Errors.Conflict.Mismatch("Product price discrepancy"));

        var purchaseResult = Purchase.Create(card.Id);
        if (purchaseResult.IsFailed)
            return Result.Fail(purchaseResult.Errors);

        var quantityResult = Quantity.Create(request.Quantity);
        if (quantityResult.IsFailed)
            return Result.Fail(quantityResult.Errors);
        
        var purchase = purchaseResult.Value;
        var addOrUpdateResult = purchase.AddOrUpdate(product, quantityResult.Value);
        if (addOrUpdateResult.IsFailed)
            return Result.Fail(addOrUpdateResult.Errors);

        var buyResult = card.Buy(purchase);
        if (buyResult.IsFailed)
            return Result.Fail(buyResult.Errors);
        
        await _unitOfWork.PurchaseRepository.AddAsync(purchase);
        var changesSaved = await _unitOfWork.SaveChangesAsync();

        //TODO Подумать над обработкой ошибок БД
        if (!changesSaved)
            return Result.Fail("Failed to save changes");

        return new CreatePurchaseResponse(card.Balance.Value, purchase.Check.Id, product.Title);
    }
}