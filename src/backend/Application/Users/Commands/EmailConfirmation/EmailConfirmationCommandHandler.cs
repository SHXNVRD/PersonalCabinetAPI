using Application.Extensions;
using Application.Services;
using Domain.Shared.Errors;
using FluentResults;
using MediatR;

namespace Application.Users.Commands.EmailConfirmation;

public class EmailConfirmationCommandHandler : IRequestHandler<EmailConfirmationCommand, Result>
{
    private readonly AppUserManager _userManager;

    public EmailConfirmationCommandHandler(AppUserManager userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> Handle(EmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Result.Fail(Errors.Conflict.NotFound("User with the specified email address was not found"));

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
            return result.ToFluentResult();

        return Result.Ok();
    }
}