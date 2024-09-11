using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Discount
    {
        public long Id { get; set; }
        public long CardId { get; set; }
        public Card? Card { get; set; }
        public long PurchaseId { get; set; }
        public Purchase? Purchase { get; set; }
        public decimal Amount { get; set; }
    }
}