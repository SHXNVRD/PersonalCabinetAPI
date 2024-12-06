using Application.Extensions;
using Application.Services;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly AppUserManager _userManager;

    public ResetPasswordCommandHandler(AppUserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Fail("User with specified email not found");

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
            return result.ToFluentResult();

        return Result.Ok();
    }
}