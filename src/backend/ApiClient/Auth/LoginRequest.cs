namespace ApiClient.Auth;

public record LoginRequest(
    string Password,
    string Login);
