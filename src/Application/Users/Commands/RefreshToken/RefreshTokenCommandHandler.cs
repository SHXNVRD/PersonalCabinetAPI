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
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            return Result.Fail(new NotFound("User with specified id not found"));

        if (!await _tokenService.VerifyUserRefreshTokenAsync(user, request.RefreshToken))
            return Result.Fail(new Conflict("Invalid refresh token"));

        var revokeResult = await _tokenService.RevokeRefreshTokenAsync(user);
        if (revokeResult.IsFailed)
            return Result.Fail(revokeResult.Errors);

        var accessToken = await _tokenService.GenerateTokenAsync(user);
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

        return Result.Ok(new AuthResponse(accessToken, refreshToken, 
            _tokenService.TokenType, _tokenService.AccessTokenExpiresInSeconds));
    }
}