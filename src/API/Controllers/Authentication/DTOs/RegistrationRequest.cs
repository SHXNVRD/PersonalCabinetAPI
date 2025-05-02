using Application.Users.Commands.Registration;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.Authentication.DTOs;

public record RegistrationRequest(        
    string PhoneNumber,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? Patronymic);

[Mapper]
public static partial class RegistrationMapper
{
    public static partial RegistrationCommand ToCommand(RegistrationRequest request);
}