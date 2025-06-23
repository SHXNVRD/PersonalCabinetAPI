using FluentResults;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<Result<CreateProductResponse>>
{
    public int CategoryId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
}