using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Domain.Models.Base;

namespace Domain.Models
{
    public class Card : Identity
    {
        public long? UserId { get; set; }
        public virtual User? User { get; set; }
        public long BonusSystemId { get; set; }
        public virtual BonusSystem? BonusSystem { get; set; }
        public virtual ICollection<Discount>? Discounts { get; set; }
        public virtual ICollection<Purchase>? Purchases { get; set; }
        public int Number { get; set; }
        public required string CodeHash { get; set; }
        public DateTime? ActivatedAt { get; set; }
        public bool IsActivated { get; set; }
    }
}