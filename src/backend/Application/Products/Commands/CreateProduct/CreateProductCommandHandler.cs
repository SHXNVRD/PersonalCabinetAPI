
using Application.Interfaces;
using Domain.Aggregates.ProductAggregate;
using Domain.Shared.ValueObjects;
using FluentResults;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var categoryResult = Category.FromId(request.CategoryId);
        if (categoryResult.IsFailed)
            return Result.Fail(categoryResult.Errors);

        var quantityResult = Quantity.Create(request.Quantity);
        if (quantityResult.IsFailed)
            return Result.Fail(quantityResult.Errors);
        
        var productResult = Product.Create(request.Title, request.Description, 
            request.Price, quantityResult.Value, categoryResult.Value);
        
        if (productResult.IsFailed)
            return Result.Fail(productResult.Errors);

        var product = productResult.Value;
        
        await _unitOfWork.ProductRepository.AddAsync(product);
        var changesSaved = await _unitOfWork.SaveChangesAsync();
        if (!changesSaved)
            return Result.Fail("Failed to save changes");

        return Result.Ok(new CreateProductResponse(product.Id));
    }
}