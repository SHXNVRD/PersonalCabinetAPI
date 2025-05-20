using Application.Users.Commands.Login;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.Authentication.DTOs;

public record LoginRequest(
        string Password,
        string Login);

[Mapper]
public static partial class LoginMapper
{
    public static partial LoginCommand ToCommand(LoginRequest request);
}