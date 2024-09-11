using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Category
    {
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public ICollection<Product>? Products { get; set; }
    }
}