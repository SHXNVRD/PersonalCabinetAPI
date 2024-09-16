using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        int AccessTokenExpiresInSeconds { get; }
        string TokenType { get; }
        Task<string> GenerateTokenAsync(User user);
        Task<string> GenerateRefreshTokenAsync(User user);
        bool TryGetPrincipal(string token, out ClaimsPrincipal claimsPrincipal);
        Task<bool> VerifyUserRefreshTokenAsync(User user, string refreshToken);
        Task<IdentityResult> RevokeRefreshTokenAsync(User user);
    }
}