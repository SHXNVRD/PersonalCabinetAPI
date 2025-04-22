using Application.Users.Commands.Registration;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.Authentication.DTOs;

public record RegistrationRequest(        
    string UserName,
    string PhoneNumber,
    string Email,
    string Password);

[Mapper]
public static partial class RegistrationMapper
{
    public static partial RegistrationCommand ToCommand(RegistrationRequest request);
}