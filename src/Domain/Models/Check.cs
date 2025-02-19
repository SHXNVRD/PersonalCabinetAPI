using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

public class Check : Identity
{
    public long? PurchaseId { get; set; }
    public virtual Purchase? Purchase { get; set; }
    
    public long? RefundId { get; set; }
    public virtual Refund? Refund { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    [NotMapped]
    public bool IsAssociatedWithPurchase => PurchaseId.HasValue;
    
    [NotMapped]
    public bool IsAssociatedWithRefund => RefundId.HasValue;
}