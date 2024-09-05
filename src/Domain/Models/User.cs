using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class User : IdentityUser<long>
    {
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public DateTime DateOfBirth { get; set; }
    }
}