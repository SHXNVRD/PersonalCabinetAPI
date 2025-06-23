using Application.Interfaces;
using Domain.Aggregates.ProductAggregate;
using Domain.Shared.Errors;
using Domain.Shared.ValueObjects;
using FluentResults;
using MediatR;

namespace Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.ProductRepository.FindById(request.Id, TrackingType.Tracking);
        if (product is null)
            return Result.Fail(Errors.Conflict.NotFound("Product with specified id not found"));

        var categoryResult = Category.FromId(request.CategoryId);
        if (categoryResult.IsFailed)
            return Result.Fail(categoryResult.Errors);

        var quantityResult = Quantity.Create(request.Quantity);
        if (quantityResult.IsFailed)
            return Result.Fail(quantityResult.Errors);

        var changeCategoryResult = product.ChangeCategory(categoryResult.Value);
        if (changeCategoryResult.IsFailed)
            return Result.Fail(changeCategoryResult.Errors);

        var changeQuantityResult = product.ChangeQuantity(quantityResult.Value);
        if (changeQuantityResult.IsFailed)
            return Result.Fail(changeQuantityResult.Errors);

        var changeTitleResult = product.ChangeTitle(request.Title);
        if (changeTitleResult.IsFailed)
            return Result.Fail(changeTitleResult.Errors);

        var changeDescriptionResult = product.ChangeDescription(request.Description);
        if (changeDescriptionResult.IsFailed)
            return Result.Fail(changeDescriptionResult.Errors);

        var changePriceResult = product.ChangePrice(request.Price);
        if (changePriceResult.IsFailed)
            return Result.Fail(changePriceResult.Errors);
        
        _unitOfWork.ProductRepository.Update(product);
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Ok();
    }
}