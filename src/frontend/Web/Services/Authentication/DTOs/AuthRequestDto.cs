namespace Web.Services.Authentication.DTOs;

public record AuthRequestDto(
    string Password,
    string? UserName,
    string? Email);