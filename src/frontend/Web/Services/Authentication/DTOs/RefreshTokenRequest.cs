namespace Web.Services.Authentication.DTOs;

public record RefreshTokenRequest(string AccessToken, string RefreshToken);