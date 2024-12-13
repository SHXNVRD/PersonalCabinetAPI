using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class User : IdentityUser<long>
    {
        public DateTime RegisteredAt { get; set; }
        public DateOnly? BirthDate { get; set; }
        public virtual ICollection<Card>? Cards { get; set; }
    }
}