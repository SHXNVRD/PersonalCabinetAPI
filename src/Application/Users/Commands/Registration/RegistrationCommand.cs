using Application.DTOs;
using Application.Users.DTOs;
using FluentResults;
using MediatR;

namespace Application.Users.Commands.Registration
{
    public record RegistrationCommand(
        string UserName,
        string PhoneNumber,
        string Email,
        string Password) : IRequest<Result>;
}