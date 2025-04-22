namespace ApiClient.Auth;

public record RegisterRequest(
    string Email,
    string PhoneNumber,
    string Password,
    string FirstName,
    string SecondName,
    string? Patronymic);