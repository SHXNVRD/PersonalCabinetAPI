using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Users.Commands.Registration;
using Domain.Models;

namespace Application.Extensions
{
    public static class UserExtensions
    {
        public static User ToEntity(this RegistrationCommand request)
        {
            return new User
            {
                Email = request.Email,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber
            };
        }
    }
}