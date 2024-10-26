using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Users.DTOs;
using Domain.Models;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Application.Users.Commands.RefreshToken
{
    public class RefreshTokenCommnadHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        
        public RefreshTokenCommnadHandler(ITokenService tokenService, UserManager<User> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
                        
            if (user == null)
                return Result.Fail($"User with specified id not found");

            if (!await _tokenService.VerifyUserRefreshTokenAsync(user, request.RefreshToken))
                return Result.Fail("Invalid refresh token");

            var revokeResult = await _tokenService.RevokeRefreshTokenAsync(user);

            if (!revokeResult.Succeeded)
                return Result.Fail("Failed to refresh token");

            var newAccessToken = await _tokenService.GenerateTokenAsync(user);
            var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

            return Result.Ok(new AuthResponse
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                TokenType = _tokenService.TokenType,
                ExpiresIn = _tokenService.AccessTokenExpiresInSeconds
            });
        }
    }
}