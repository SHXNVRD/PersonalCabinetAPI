using Application.Users.Commands.SendPasswordResetLink;
using Riok.Mapperly.Abstractions;

namespace API.DTOs.AccountController;

public record SendPasswordResetLinkRequest(string Email);

[Mapper]
public static partial class SendPasswordResetLinkMapper
{
    public static partial SendPasswordResetLinkCommand ToCommand(SendPasswordResetLinkRequest request);
}