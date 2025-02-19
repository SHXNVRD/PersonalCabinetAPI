using Domain.Models.Base;

namespace Domain.Models;

public class RefundItem : Identity
{
    public long RefundId { get; set; }
    public virtual Refund? Refund { get; set; }
    public long ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public required int Quantity { get; set; }
    public decimal Total { get; set; }
}