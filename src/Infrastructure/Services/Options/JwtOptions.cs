namespace Infrastructure.Services.Options
{
    public class JwtOptions
    {
        public required string Issuer { get; init; }
        public required string Audience { get; init; }
        public required int AccessTokenExpiresInSeconds { get; init; }
        public required string TokenType { get; init; }
        public required string Key { get; init; }
    }
}