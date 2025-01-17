using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Riok.Mapperly.Abstractions;

namespace Application.Users.DTOs
{
    public class UserResponse
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public DateOnly? BirthDate { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}