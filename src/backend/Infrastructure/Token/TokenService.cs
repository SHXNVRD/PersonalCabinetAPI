using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces.Token;
using Domain.Aggregates.UserAggregate;
using Domain.Shared.Errors;
using FluentResults;
using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Token;

public class TokenService : ITokenService
{
    private const string RefreshTokenPurpose = "Refresh";
    private readonly JwtOptions _jwtOptions;
    private readonly SigningCredentials _signingCredentials;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<TokenService> _logger;
    public int AccessTokenExpiresInSeconds => _jwtOptions.AccessTokenExpiresInSeconds;
    public string TokenType => _jwtOptions.TokenType;
    public TokenService(
        IOptions<JwtOptions> jwtOptions, 
        UserManager<User> userManager, 
        ILogger<TokenService> logger)
    {
        _jwtOptions = jwtOptions.Value;
        _userManager = userManager;
        _logger = logger;

        var keyBytes = Encoding.UTF8.GetBytes(_jwtOptions.Key);
        _signingCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes),
            SecurityAlgorithms.HmacSha256Signature);
    }

    public async Task<string> GenerateRefreshTokenAsync(User user)
    {
        var refreshToken = await _userManager.GenerateUserTokenAsync(user, TokenProvider.RefreshProvider, RefreshTokenPurpose);

        await _userManager.SetAuthenticationTokenAsync(user, TokenProvider.RefreshProvider, RefreshTokenPurpose, refreshToken);

        return refreshToken;
    }

    public bool TryGetPrincipal(string token, out ClaimsPrincipal claimsPrincipal)
    {
        claimsPrincipal = new ClaimsPrincipal();
        
        var tokenHandler = new JwtSecurityTokenHandler();
        if (!tokenHandler.CanReadToken(token))
            return false;
        
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = _jwtOptions.Audience,
            ValidateIssuer = true,
            ValidIssuer = _jwtOptions.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key)),
            ValidateLifetime = false
        };

        SecurityToken securityToken;
        
        try
        {
            claimsPrincipal = tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out securityToken);
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to validate token. Reason: {Reason}", e.Message);
            return false;
        }

        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            return false;
            
        return true;
    }

    public async Task<Result> RevokeRefreshTokenAsync(User user)
    {
        var revokeResult = await _userManager.RemoveAuthenticationTokenAsync(user, TokenProvider.RefreshProvider, RefreshTokenPurpose);
        if (!revokeResult.Succeeded)
            return Result.Fail(new Conflict(revokeResult.Errors.First().Description));

        var updateResult = await _userManager.UpdateSecurityStampAsync(user);
        if (!updateResult.Succeeded)
            return Result.Fail(new Conflict(updateResult.Errors.First().Description));

        return Result.Ok();
    }

    public async Task<bool> VerifyUserRefreshTokenAsync(User user, string refreshToken) => 
        await _userManager.VerifyUserTokenAsync(user, TokenProvider.RefreshProvider, RefreshTokenPurpose, refreshToken);

    public async Task<string> GenerateTokenAsync(User user)
    {
        var handler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Issuer =  _jwtOptions.Issuer,
            Subject = await GenerateClaimsAsync(user),
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddSeconds(_jwtOptions.AccessTokenExpiresInSeconds),
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = _signingCredentials
        };
            
        var token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }

    private async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = new ClaimsIdentity();

        var id = Guid
            .NewGuid()
            .ToString()
            .GetHashCode()
            .ToString("x", CultureInfo.InvariantCulture);    

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, id),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Aud, _jwtOptions.Audience),
            new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Issuer),
        };

        var roles = await _userManager.GetRolesAsync(user);
            
        foreach (var role in roles)
            identity.AddClaim(new Claim(ClaimTypes.Role, role));

        identity.AddClaims(claims);
            
        return identity;
    }
}