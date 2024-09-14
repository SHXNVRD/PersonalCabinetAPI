using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Product
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public Category? Category { get; set; }
        public string Title { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public ICollection<PurchaseItem>? PurchaseItems { get; set; }
    }
}