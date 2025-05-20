using Application.Users.Commands.CreateEmailConfirmationLink;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.Authentication.DTOs;

public record CreateEmailConfirmationLinkRequest(string Email, string? RedirectUrl);

[Mapper]
public partial class CreateEmailConfirmationLinkMapper
{
    public static partial CreateEmailConfirmationLinkCommand ToCommand(
        CreateEmailConfirmationLinkRequest request);
}