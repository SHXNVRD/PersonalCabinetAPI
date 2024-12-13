using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models.Base;

namespace Domain.Models
{
    public class PurchaseItem : Identity
    {
        public long PurchaseId { get; set; }
        public virtual Purchase? Purchase { get; set; }
        public long ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public required int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}