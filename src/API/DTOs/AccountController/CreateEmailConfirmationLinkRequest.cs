using Application.Users.Commands.CreateEmailConfirmationLink;
using MediatR;
using Riok.Mapperly.Abstractions;

namespace API.DTOs.AccountController;

public record CreateEmailConfirmationLinkRequest(string Email);

[Mapper]
public partial class CreateEmailConfirmationLinkMapper
{
    public static partial CreateEmailConfirmationLinkCommand ToCommand(
        CreateEmailConfirmationLinkRequest request);
}