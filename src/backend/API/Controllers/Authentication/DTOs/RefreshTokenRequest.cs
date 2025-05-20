using Application.Users.Commands.RefreshToken;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.Authentication.DTOs;
public record RefreshTokenRequest(string AccessToken, string RefreshToken);

[Mapper]
public partial class RefreshTokenMapper
{
    public static partial RefreshTokenCommand ToCommand(RefreshTokenRequest request);
}