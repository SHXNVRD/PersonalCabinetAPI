using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models.Base;

namespace Domain.Models
{
    public class Category : Identity
    {
        public required string Title { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
    }
}