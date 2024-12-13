using FluentResults;
using MediatR;

namespace Application.Users.Commands.EmailConfirmation
{
    public class EmailConfirmationCommand : IRequest<Result>
    {
        public string Email { get; set; }
        public string Token { get; set; }
    }
}