using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Application.Options;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
    public class TokenService : ITokenService
    {
        private const string RefreshTokenPurpose = "RefreshToken";
        private readonly JwtOptions _jwtOptions;
        private readonly SigningCredentials _signingCredentials;
        private readonly UserManager<User> _userManager;
        public int AccessTokenExpiresInSeconds { get => _jwtOptions.AccessTokenExpiresInSeconds; } 
        public string TokenType { get => _jwtOptions.TokenType; }
        public TokenService(IOptions<JwtOptions> jwtOptions, UserManager<User> userManager)
        {
            _jwtOptions = jwtOptions.Value;
            _userManager = userManager;

            var keyBytes = Encoding.UTF8.GetBytes(_jwtOptions.Key);
            _signingCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature);
        }

        public async Task<string> GenerateRefreshTokenAsync(User user)
        {
            var refreshToken = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, RefreshTokenPurpose);

            await _userManager.SetAuthenticationTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, RefreshTokenPurpose, refreshToken);

            return refreshToken;
        }

        public bool TryGetPrincipal(string token, out ClaimsPrincipal claimsPrincipal)
        {
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

            var tokenHandler = new JwtSecurityTokenHandler();
            claimsPrincipal = tokenHandler.ValidateToken(
                token,
                tokenValidationParameters,
                out SecurityToken securityToken);
                
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return false;
            
            return true;
        }

        public async Task<IdentityResult> RevokeRefreshTokenAsync(User user)
        {
            var revokeResult = await _userManager.RemoveAuthenticationTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, RefreshTokenPurpose);
            
            if (!revokeResult.Succeeded)
                return revokeResult;

            return await _userManager.UpdateSecurityStampAsync(user);
        }

        public async Task<bool> VerifyUserRefreshTokenAsync(User user, string refreshToken) => 
            await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, RefreshTokenPurpose, refreshToken);

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
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Issuer),
            };

            var roles = await _userManager.GetRolesAsync(user);
            
            foreach (var role in roles)
                identity.AddClaim(new Claim(ClaimTypes.Role, role));

            identity.AddClaims(claims);
            
            return identity;
        }
    }
}