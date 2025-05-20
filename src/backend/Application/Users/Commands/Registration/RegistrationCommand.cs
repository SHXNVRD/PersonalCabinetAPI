using Application.DTOs;
using Application.Users.DTOs;
using FluentResults;
using MediatR;

namespace Application.Users.Commands.Registration;

public class RegistrationCommand : IRequest<Result>
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Patronymic { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}