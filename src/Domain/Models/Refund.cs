using Domain.Models.Base;

namespace Domain.Models;

public class Refund : Identity
{
    public virtual Check? Check { get; set; }
    public long PurchaseId { get; set; }
    public virtual Purchase? Purchase { get; set; }
    public decimal Total => RefundItems.Sum(ri => ri.Total);
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<RefundItem> RefundItems { get; set; } = [];
}