namespace Web.Services.Authentication.DTOs;

public record AuthResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    string RefreshToken);