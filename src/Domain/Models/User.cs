using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class User : IdentityUser<long>
    {
        public DateTime RegistrationDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ICollection<Card>? Cards { get; set; }
    }
}