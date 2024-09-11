using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Purchase
    {
        public long Id { get; set; }
        public long StationId { get; set; }
        public Station? Station { get; set; }
        public long? CardId { get; set; }
        public Card? Card { get; set; }
        public ICollection<PurchaseItem> PurchaseItems { get; set; } = [];
        public Discount? Discount { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
    }
}