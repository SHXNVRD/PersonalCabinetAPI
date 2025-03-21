using Application.Users.Commands.Login;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.Account.DTOs;

public record LoginRequest(
    string Email,
    string Password);

[Mapper]
public static partial class LoginMapper
{
    public static partial LoginCommand ToCommand(LoginRequest request);
}