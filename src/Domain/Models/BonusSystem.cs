using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models.Base;

namespace Domain.Models
{
    public class BonusSystem : Identity
    {
        public required string Title { get; set; }
        public float DiscountPercent { get; set; }
        public virtual ICollection<Card>? Cards { get; set; }
    }
}