namespace Web.Services.Authentication.DTOs;

public record RegisterRequestDto(
    string FirstName,
    string SecondName,
    string Email,
    string PhoneNumber,
    string Password,
    string? Patronymic);