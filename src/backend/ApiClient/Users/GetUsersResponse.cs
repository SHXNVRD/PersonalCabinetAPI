namespace ApiClient.Users;

public record GetUsersResponse(UserViewModel[] Users);

public record UserViewModel(
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
