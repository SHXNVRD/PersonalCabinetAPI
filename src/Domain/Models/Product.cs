using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models.Base;

namespace Domain.Models
{
    public class Product : Identity
    {
        public long CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public required string Title { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = [];
        public virtual ICollection<RefundItem>? RefundItems { get; set; } = [];
        public virtual ICollection<ProductPriceHistory> ProductPriceHistories { get; set; } = [];
        public decimal Price => ProductPriceHistories
            .OrderByDescending(pph => pph.ChangedAt)
            .Select(pph => pph.Price)
            .First();
    }
}