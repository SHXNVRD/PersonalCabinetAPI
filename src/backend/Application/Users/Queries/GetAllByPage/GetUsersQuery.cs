using FluentResults;
using MediatR;

namespace Application.Users.Queries.GetAllByPage;

public class GetUsersQuery : IRequest<Result<GetUsersResponse>>
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}

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