using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models.Base;

namespace Domain.Models
{
    public class Discount : Identity
    {
        public long CardId { get; set; }
        public virtual Card? Card { get; set; }
        public long PurchaseId { get; set; }
        public virtual Purchase? Purchase { get; set; }
        public decimal Amount { get; set; }
    }
}