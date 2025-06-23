using Application.Interfaces;
using Domain.Shared.Errors;
using FluentResults;
using MediatR;

namespace Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var isDeleted = await _unitOfWork.ProductRepository.RemoveAsync(request.Id);
        if (!isDeleted)
            return Errors.Conflict.NotFound("Product with specified id not found");

        return Result.Ok();
    }
}