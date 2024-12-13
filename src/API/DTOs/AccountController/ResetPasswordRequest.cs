using Application.Users.Commands.ResetPassword;
using Riok.Mapperly.Abstractions;

namespace API.DTOs.AccountController;

public record ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword);

[Mapper]
public static partial class ResetPasswordMapper
{
    public static partial ResetPasswordCommand ToCommand(ResetPasswordRequest request);
}