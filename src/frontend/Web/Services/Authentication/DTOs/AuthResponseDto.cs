namespace Web.Services.Authentication.DTOs;

public record AuthResponseDto(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    string RefreshToken);