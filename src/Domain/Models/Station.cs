using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Station
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Address { get; set; } = null!;
        public ICollection<Purchase>? Purchases { get; set; }
    }
}