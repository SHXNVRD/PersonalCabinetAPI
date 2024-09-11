using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Interfaces;
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
            var emailClaim = _tokenService.GetPrincipal(request.AccessToken)
                                .Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            var user =  await _userManager.FindByEmailAsync(emailClaim.Value);
            
            if (!await _tokenService.VerifyUserRefreshTokenAsync(user, request.RefreshToken))
                return Result.Fail("Invalid refresh token");

            var revokeResult = await _tokenService.RevokeRefreshTokenAsync(user);

            if (!revokeResult.Succeeded)
                return Result.Fail("Failed to refresh the token");

            var newToken = await _tokenService.GenerateTokenAsync(user);
            var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync(user);

            return Result.Ok(new AuthResponse
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                TokenType = _tokenService.TokenType,
                ExpiresIn = _tokenService.AccessTokenExpiresInSeconds
            });
        }
    }
}