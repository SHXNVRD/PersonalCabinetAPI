using Application.DTOs;
using Application.Users.DTOs;
using FluentResults;
using MediatR;

namespace Application.Users.Commands.Registration
{
    public class RegistrationCommand : IRequest<Result>
    {
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}