using API.Requests;
using Application.Users.Commands.ResetPassword;

namespace API.Extensions.RequestsExtensions;

public static class ResetPasswordRequestExtensions
{
    public static ResetPasswordCommand ToCommand(this ResetPasswordRequest request) =>
        new ResetPasswordCommand(request.Email, request.Token, request.NewPassword);
}