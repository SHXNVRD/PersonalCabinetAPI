using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models.Base;

namespace Domain.Models
{
    public class Purchase : Identity
    {
        public long StationId { get; set; }
        public virtual Station? Station { get; set; }
        public long? CardId { get; set; }
        public virtual Card? Card { get; set; }
        public virtual Refund? Refund { get; set; }
        public virtual Check? Check { get; set; }
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public decimal Total { get; set; }
    }
}