using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class BonusSystem
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public float DiscountPercent { get; set; }
        public ICollection<Card>? Cards { get; set; }
    }
}