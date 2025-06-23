using Application.Users.Commands.SendPasswordResetLink;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.Authentication.DTOs;

public record SendPasswordResetLinkRequest(string Email);

[Mapper]
public static partial class SendPasswordResetLinkMapper
{
    public static partial SendPasswordResetLinkCommand ToCommand(SendPasswordResetLinkRequest request);
}