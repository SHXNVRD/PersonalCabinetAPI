using Application.Extensions;
using Application.Services;
using Domain.Shared.Errors;
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
            return Result.Fail(new NotFound("User with specified email not found"));

        var resetResult = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!resetResult.Succeeded)
            return Result.Fail(new Conflict(resetResult.Errors.First().Description));

        return Result.Ok();
    }
}