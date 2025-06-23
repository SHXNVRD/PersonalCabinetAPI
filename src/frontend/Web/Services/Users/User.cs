namespace Web.Services.Users;

public record User(
    Guid Id,
    string UserName,
    string Name,
    DateOnly? DayOfBirth,
    string PhoneNumber,
    bool PhoneNumberConfirmed,
    string Email,
    bool EmailConfirmed,
    bool TwoFactorEnabled,
    DateTime RegisteredAt,
    bool IsBlocked,
    int AccessFailedCount);