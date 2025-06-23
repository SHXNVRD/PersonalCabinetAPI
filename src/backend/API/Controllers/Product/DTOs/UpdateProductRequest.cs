using Application.Products.Commands.UpdateProduct;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.Product.DTOs;

public record UpdateProductRequest(
    int CategoryId,
    string Title,
    string Description,
    decimal Quantity,
    decimal Price);

[Mapper]
public static partial class UpdateProductMapper
{
    [MapperIgnoreTarget("Id")]
    public static partial UpdateProductCommand ToCommand(UpdateProductRequest request);
}