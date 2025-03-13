using Application.Interfaces;
using Application.Purchases.DTOs;
using Domain.Aggregates;
using Domain.Aggregates.PurchaseAggregate;
using Domain.Helpers;
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
        var card = await _unitOfWork.CardRepository.FindByNumberAsync(request.CardNumber, TrackingType.Tracking);

        if (card == null)
            return Result.Fail($"Card with number {request.CardNumber} was not found");

        if (await Hasher.ComputeSha256HashAsync(request.PinCode) != card.PinCodeHash)
            return Result.Fail("Incorrect card pin-code");

        var product = await _unitOfWork.ProductRepository.FindById(request.ProductId);

        if (product == null)
            return Result.Fail($"Product with id {request.ProductId} was not found");
        
        Check check = new()
        {
            CreatedAt = request.CreatedAt
        };

        Purchase purchase = new()
        {
            CardId = card.Id,
            CreatedAt = request.CreatedAt,
            Check = check,
            PurchaseItems = new List<PurchaseItem>()
            {
                new()
                {
                    ProductPriceAtPurchase = product.Price,
                    ProductId = product.Id,
                    Quantity = request.Quantity
                }
            }
        };

        card.Balance -= purchase.Total;

        await _unitOfWork.PurchaseRepository.AddAsync(purchase);
        var changesSaved = await _unitOfWork.SaveChangesAsync();

        if (!changesSaved)
            return Result.Fail("Failed to save changes");

        return new CreatePurchaseResponse()
        {
            CardBalance = card.Balance,
            ProductName = product.Title,
            CheckId = check.Id
        };
    }
}