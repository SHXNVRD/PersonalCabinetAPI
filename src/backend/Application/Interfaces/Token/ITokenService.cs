using System.Security.Claims;
using Domain.Aggregates.UserAggregate;
using FluentResults;

namespace Application.Interfaces.Token;

public interface ITokenService
{
    int AccessTokenExpiresInSeconds { get; }
    string TokenType { get; }
    Task<string> GenerateTokenAsync(User user);
    Task<string> GenerateRefreshTokenAsync(User user);
    bool TryGetPrincipal(string token, out ClaimsPrincipal claimsPrincipal);
    Task<bool> VerifyUserRefreshTokenAsync(User user, string refreshToken);
    Task<Result> RevokeRefreshTokenAsync(User user);
}