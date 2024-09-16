using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs;
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

        public static UserResponse ToDto(this User user)
        {
            return new UserResponse()
            {
                Id = user.Id,
                Name = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                DateOfBirth = user.DateOfBirth
            };
        }
    }
}