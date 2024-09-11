using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Options
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int AccessTokenExpiresInSeconds { get; set; }
        public string TokenType { get; set; } = null!;
        public string Key { get; set; } = null!;
    }
}