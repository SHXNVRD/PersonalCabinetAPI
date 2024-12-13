using Application.Users.Commands.RefreshToken;
using Riok.Mapperly.Abstractions;

namespace API.DTOs.AccountController;
public record RefreshTokenRequest(string RefreshToken);

[Mapper]
public partial class RefreshTokenMapper
{
    [MapperIgnoreTarget(nameof(RefreshTokenCommand.UserId))]
    public static partial RefreshTokenCommand ToCommand(RefreshTokenRequest request);
}