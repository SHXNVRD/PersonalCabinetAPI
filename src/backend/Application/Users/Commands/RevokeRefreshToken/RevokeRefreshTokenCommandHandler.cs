using Application.Interfaces.Token;
using Domain.Aggregates.UserAggregate;
using Domain.Shared.Errors;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Users.Commands.RevokeRefreshToken;

public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, Result>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public RevokeRefreshTokenCommandHandler(ITokenService tokenService, UserManager<User> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task<Result> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return Result.Fail(Errors.Conflict.NotFound("User with specified id not found"));

        var revokeResult = await _tokenService.RevokeRefreshTokenAsync(user);
        if (revokeResult.IsFailed)
            return Result.Fail(revokeResult.Errors);

        return Result.Ok();
    }
}