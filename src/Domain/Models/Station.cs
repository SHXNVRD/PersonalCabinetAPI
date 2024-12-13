using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models.Base;

namespace Domain.Models
{
    public class Station : Identity
    {
        public string Title { get; set; } = null!;
        public string Address { get; set; } = null!;
        public ICollection<Purchase>? Purchases { get; set; }
    }
}