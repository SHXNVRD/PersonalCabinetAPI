namespace Web.Services.Authentication.DTOs;

public record AuthRequest(
    string Password, 
    string Login);