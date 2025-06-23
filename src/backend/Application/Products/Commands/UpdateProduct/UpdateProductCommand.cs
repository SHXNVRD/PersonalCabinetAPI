using FluentResults;
using MediatR;

namespace Application.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<Result>
{
    public long Id { get; set; }
    public int CategoryId { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
}