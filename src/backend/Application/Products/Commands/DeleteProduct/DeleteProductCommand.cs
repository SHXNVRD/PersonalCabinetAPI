using FluentResults;
using MediatR;

namespace Application.Products.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest<Result>
{
    public long Id { get; set; }
}