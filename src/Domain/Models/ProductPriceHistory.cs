using Domain.Models.Base;

namespace Domain.Models;

public class ProductPriceHistory : Identity
{
    public long ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public required decimal Price { get; set; }
    public DateTime ChangedAt { get; set; }
}