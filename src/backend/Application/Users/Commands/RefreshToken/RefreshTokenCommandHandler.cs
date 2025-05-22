using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Token;
using Application.Users.DTOs;
using Domain.Aggregates.UserAggregate;
using Domain.Shared.Errors;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Application.Users.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<User> _userManager;
        
    public RefreshTokenCommandHandler(ITokenService tokenService, UserManager<User> userManager)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (!_tokenService.TryGetPrincipal(request.AccessToken, out var claimsPrincipal))
            return Result.Fail(Errors.Unauthorized.WrongCredentials("Invalid access token"));

        var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return Result.Fail(Errors.Unauthorized.WrongCredentials("Access token doesn`t contain require claims"));
        
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Fail(Errors.NotFound.EntityNotFound("User with specified id not found"));

        if (!await _tokenService.VerifyUserRefreshTokenAsync(user, request.RefreshToken))
            return Result.Fail(Errors.Unauthorized.WrongCredentials("Invalid refresh token"));
        
        var revokeResult = await _tokenService.RevokeRefreshTokenAsync(user);
        if (revokeResult.IsFailed)
            return Result.Fail(Errors.Unauthorized
                .FailedToRefreshToken()
                .CausedBy(revokeResult.Errors));

        var accessToken = await _tokenService.GenerateTokenAsync(user);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

        return Result.Ok(new AuthResponse(accessToken, refreshToken, 
            _tokenService.TokenType, _tokenService.AccessTokenExpiresInSeconds));
    }
}