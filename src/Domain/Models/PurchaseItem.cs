using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class PurchaseItem
    {
        public long Id { get; set; }
        public long PurchaseId { get; set; }
        public Purchase? Purchase { get; set; }
        public long ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}