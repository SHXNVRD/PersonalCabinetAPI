using FluentResults;
using MediatR;

namespace Application.Users.Commands.CreateEmailConfirmationLink
{
    public class CreateEmailConfirmationLinkCommand : IRequest<Result>
    {
        public string Email { get; set; }
    }
}