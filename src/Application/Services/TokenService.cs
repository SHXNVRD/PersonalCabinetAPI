using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Interfaces;
using Application.Options;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly SigningCredentials _signingCredentials;
        public TokenService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;

            var keyBytes = Encoding.UTF8.GetBytes(_jwtOptions.Key);
            _signingCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature);
        }

        public string GenerateToken(User user,  bool isAdmin = false)
        {
            var handler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer =  _jwtOptions.Issuer,
                Subject = GenerateClaims(user, isAdmin),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddSeconds(_jwtOptions.ExpiresSeconds),
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = _signingCredentials,
            };
            
            var token = handler.CreateToken(tokenDescriptor);

            return handler.WriteToken(token);
        }

        private ClaimsIdentity GenerateClaims(User user, bool isAdmin = false)
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
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Aud, _jwtOptions.Audience),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Iss, _jwtOptions.Issuer)
            };

            if (isAdmin)
                identity.AddClaim(new Claim(ClaimTypes.Role, "admin"));
            else
                identity.AddClaim(new Claim(ClaimTypes.Role, "user"));

            identity.AddClaims(claims);
            
            return identity;
        }
    }
}