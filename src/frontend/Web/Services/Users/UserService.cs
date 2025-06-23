using ApiClient;
using FluentResults;

namespace Web.Services.Users;

public class UserService
{
    private readonly GasStationClient _client;

    public UserService(GasStationClient client)
    {
        _client = client;
    }

    public async Task<Result<IEnumerable<User>>> GetAllAsync()
    {
        var apiResponse = await _client.UserClient.GetAllAsync();

        if (!apiResponse.IsSuccessStatusCode)
            return Result.Fail(apiResponse.GetError()?.Errors.FirstOrDefault().Value[0] 
                               ?? "Не удалось получить список пользователей");

        var usersList = apiResponse.Content?.Users.Select(u => new User(
            u.Id,
            u.UserName,
            u.Name,
            u.DayOfBirth,
            u.PhoneNumber,
            u.PhoneNumberConfirmed,
            u.Email,
            u.EmailConfirmed,
            u.TwoFactorEnabled,
            u.RegisteredAt,
            u.IsBlocked,
            u.AccessFailedCount)) ?? [];

        return Result.Ok(usersList);
    }
}