using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Cards.DTOs
{
    public class CardResponse
    {
        public long Id { get; set; }
        public bool IsActivated { get; set; }
        public int Number { get; set; }
        public float DiscountPercent { get; set; }
        public string BonusSystemTitle { get; set; } = null!;
    }
}