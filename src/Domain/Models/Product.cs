using System;
using System.Collections.Generic;
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
        public required decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<PurchaseItem>? PurchaseItems { get; set; }
    }
}