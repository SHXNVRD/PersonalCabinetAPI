using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Card
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public User? User { get; set; }
        public long BonusSystemId { get; set; }
        public BonusSystem BonusSystem { get; set; } = null!;
        public ICollection<Discount>? Discounts { get; set; }
        public ICollection<Purchase>? Purchases { get; set; }
        public int Number { get; set; }
        public string CodeHash { get; set; } = null!;
        public DateTime? ActivationDate { get; set; }
        public bool IsActivated { get; set; }
    }
}