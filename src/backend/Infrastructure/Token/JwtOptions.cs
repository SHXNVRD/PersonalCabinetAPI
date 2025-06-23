namespace Infrastructure.Token;

public class JwtOptions
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required int AccessTokenExpiresInSeconds { get; set; }
    public required string TokenType { get; set; }
    public required string Key { get; set; }
}