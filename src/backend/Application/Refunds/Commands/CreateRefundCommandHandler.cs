using Application.Interfaces;
using Domain.Shared.Errors;
using Domain.Shared.ValueObjects;
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
        var cardNumberResult = CardNumber.Create(request.CardNumber);
        if (cardNumberResult.IsFailed)
            return Result.Fail(cardNumberResult.Errors);
        
        var card = await _unitOfWork.CardRepository.FindByNumberWithPurchasesAndRefundsAsync(cardNumberResult.Value, TrackingType.Tracking);

        if (card == null)
            return Result.Fail(Errors.Conflict.NotFound($"Card with number {request.CardNumber} was not found"));

        var quantityResult = Quantity.Create(request.Quantity);
        if (quantityResult.IsFailed)
            return Result.Fail(quantityResult.Errors);

        var refundResult = card.ReturnLatest(request.ProductId, request.ProductPrice, quantityResult.Value);
        if (refundResult.IsFailed)
            return Result.Fail(refundResult.Errors);

        await _unitOfWork.RefundRepository.AddAsync(refundResult.Value);
        var changesSaved = await _unitOfWork.SaveChangesAsync();

        if (!changesSaved)
            return Result.Fail("Failed to save changes");

        var product = card.Purchases.MaxBy(p => p.CreatedAt)!.PurchaseItems[0].Product;

        return new CreateRefundResponse(card.Balance.Value, refundResult.Value.Check.Id, product.Title);
    }
}