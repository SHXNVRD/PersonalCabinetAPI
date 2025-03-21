using Application.Users.Commands.CreateEmailConfirmationLink;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.Account.DTOs;

public record CreateEmailConfirmationLinkRequest(string Email);

[Mapper]
public partial class CreateEmailConfirmationLinkMapper
{
    public static partial CreateEmailConfirmationLinkCommand ToCommand(
        CreateEmailConfirmationLinkRequest request);
}