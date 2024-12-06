using API.Requests;
using Application.Users.Commands.RefreshToken;
using Application.Users.Commands.SendPasswordResetLink;

namespace API.Extensions.RequestsExtensions;

public static class SendPasswordResetLinkRequestExtensions
{
    public static SendPasswordResetLinkCommand ToCommand(this SendPasswordResetLinkRequest request) =>
        new SendPasswordResetLinkCommand(request.Email);
}