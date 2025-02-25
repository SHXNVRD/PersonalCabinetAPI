using System.Numerics;
using Application.Helpers;
using Application.Interfaces;
using Application.Refunds.DTOs;
using Domain.Models;
using FluentResults;
using MediatR;

namespace Application.Refunds.Commands;

public class CreateRefundCommandHandler : IRequestHandler<CreateRefundCommand, Result<CreateRefundResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateRefundCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateRefundResponse>> Handle(CreateRefundCommand request, CancellationToken cancellationToken)
    {
        var refundingPurchase = await _unitOfWork.PurchaseRepository.GetLatestByCardNumber(request.CardNumber, TrackingType.Tracking);

        if (refundingPurchase == null)
            return Result.Fail($"Purchase with card number {request.CardNumber} was not found");

        var card = refundingPurchase.Card;
        
        if (await Hasher.ComputeSha256HashAsync(request.CardPinCode) != card!.PinCodeHash)
            return Result.Fail("Incorrect card pin-code");

        // Возвращаем только первый продукт. В запросе на возврат приходят данные только об одном продукте
        var refundingPurchaseItem = refundingPurchase.PurchaseItems.First();
        
        if (refundingPurchaseItem.ProductId != request.ProductId
            || refundingPurchaseItem.Quantity != request.Quantity
            || refundingPurchaseItem.Total != request.Total)
            return Result.Fail("Cannot refund non-last purchase");

        var productPrice = await _unitOfWork.ProductRepository
            .GetPriceAtDateAsync(refundingPurchaseItem.ProductId,refundingPurchase.CreatedAt);

        Check check = new()
        {
            CreatedAt = request.RefundedAt
        };
        
        Refund refund = new()
        {
            PurchaseId = refundingPurchase.Id,
            Check = check,
            RefundItems = new List<RefundItem>()
            {
                new()
                {
                    ProductId = refundingPurchaseItem.ProductId,
                    Quantity = request.Quantity,
                    ProductPriceAtRefund = productPrice
                }
            }
        };

        card.Balance += refund.Total;

        await _unitOfWork.RefundRepository.AddAsync(refund);
        var changesSaved = await _unitOfWork.SaveChangesAsync();

        if (!changesSaved)
            return Result.Fail("Failed to save changes");

        return new CreateRefundResponse()
        {
            CardBalance = card.Balance,
            ProductName = refundingPurchaseItem.Product!.Title,
            CheckId = check.Id
        };
    }
}