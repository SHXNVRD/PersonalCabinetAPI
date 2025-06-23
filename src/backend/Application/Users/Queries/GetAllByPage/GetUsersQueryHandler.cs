using Dapper;
using FluentResults;
using MediatR;
using Npgsql;

namespace Application.Users.Queries.GetAllByPage;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<GetUsersResponse>>
{ 
    private readonly NpgsqlDataSource _dataSource;

    public GetUsersQueryHandler(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<Result<GetUsersResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        if (request is { Page: { } page, PageSize: { } pageSize })
            return await GetUsers(connection, page, pageSize, cancellationToken);
        
        return await GetUsers(connection, cancellationToken);
    }

    private async Task<GetUsersResponse> GetUsers(NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        var usersList = await connection.QueryAsync<DapperUserModel>(GetUsersSql);

        var viewList = usersList.Select(u => new UserViewModel(
                u.Id,
                u.UserName,
                string.Join(' ', u.Patronymic is null
                    ? [u.FirstName, u.LastName]
                    : [u.FirstName, u.LastName, u.Patronymic]),
                u.DayOfBirth,
                u.PhoneNumber,
                u.PhoneNumberConfirmed,
                u.Email,
                u.EmailConfirmed,
                u.TwoFactorEnabled,
                u.RegisteredAt,
                u.LockoutEnd.HasValue,
                u.AccessFailedCount))
            .ToArray();

        return new GetUsersResponse(viewList);
    }

    private async Task<GetUsersResponse> GetUsers(NpgsqlConnection connection, int page, int pageSize, CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(GetUsersOffsetSql, 
            new
            {
                Offset = --page * pageSize,
                Limit = pageSize
            },
            cancellationToken: cancellationToken);

        var usersList = await connection.QueryAsync<DapperUserModel>(command);

        var viewList = usersList.Select(u => new UserViewModel(
                u.Id,
                u.UserName,
                string.Join(' ', u.Patronymic is null
                    ? [u.FirstName, u.LastName]
                    : [u.FirstName, u.LastName, u.Patronymic]),
                u.DayOfBirth,
                u.PhoneNumber,
                u.PhoneNumberConfirmed,
                u.Email,
                u.EmailConfirmed,
                u.TwoFactorEnabled,
                u.RegisteredAt,
                u.LockoutEnd.HasValue,
                u.AccessFailedCount))
            .ToArray();

        return new GetUsersResponse(viewList);
    }
    
    private const string GetUsersSql =
        """
          SELECT
              u.id AS Id,
              u.user_name AS UserName,
              u.firstname as FirstName,
              u.lastname as LastName,
              u.patronymic as Patronymic,
              u.day_of_birth AS DayOfBirth,
              u.email AS Email,
              u.email_confirmed AS EmailConfirmed,
              u.phone_number AS PhoneNumber,
              u.phone_number_confirmed AS PhoneNumberConfirmed,
              u.two_factor_enabled AS TwoFactorEnabled,
              u.registered_at AS RegisteredAt,
              u.lockout_end AS LockoutEnd,
              u.access_failed_count AS AccessFailedCount
          FROM users AS u
        """;

    private const string GetUsersOffsetSql =
        """
          SELECT
              u.id AS Id,
              u.user_name AS UserName,
              u.firstname as FirstName,
              u.lastname as LastName,
              u.patronymic as Patronymic,
              u.day_of_birth AS DayOfBirth,
              u.email AS Email,
              u.email_confirmed AS EmailConfirmed,
              u.phone_number AS PhoneNumber,
              u.phone_number_confirmed AS PhoneNumberConfirmed,
              u.two_factor_enabled AS TwoFactorEnabled,
              u.registered_at AS RegisteredAt,
              u.lockout_end AS LockoutEnd,
              u.access_failed_count AS AccessFailedCount
          FROM users AS u
          OFFSET @Offset
          LIMIT @Limit
        """;
    
    private class DapperUserModel
    {
        public Guid Id { get; init; }
        public string UserName { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string? Patronymic { get; init; }  
        public DateOnly? DayOfBirth { get; init; }
        public string Email { get; init; }
        public bool EmailConfirmed { get; init; }
        public string PhoneNumber { get; init; }
        public bool PhoneNumberConfirmed { get; init; }
        public bool TwoFactorEnabled { get; init; }
        public DateTime RegisteredAt { get; init; }
        public DateTime? LockoutEnd { get; init; }
        public int AccessFailedCount { get; init; }
    }
}