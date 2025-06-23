using Application.Products.Commands.CreateProduct;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.Product.DTOs;

public record CreateProductRequest(
    int CategoryId,
    string Title,
    string Description,
    decimal Quantity,
    decimal Price);

[Mapper]
public static partial class CreateProductMapper
{
    public static partial CreateProductCommand ToCommand(CreateProductRequest request);
}