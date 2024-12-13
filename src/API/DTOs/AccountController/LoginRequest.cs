using Application.Users.Commands.Login;
using Riok.Mapperly.Abstractions;

namespace API.DTOs.AccountController;

public record LoginRequest(
    string Email,
    string Password);

[Mapper]
public static partial class LoginMapper
{
    public static partial LoginCommand ToCommand(LoginRequest request);
}