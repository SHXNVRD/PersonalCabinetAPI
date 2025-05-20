using Application.Cards.DTOs;
using Dapper;
using Domain.Shared.Errors;
using FluentResults;
using MediatR;
using Npgsql;

namespace Application.Users.Queries.GetById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResponse>>
{
    private readonly NpgsqlDataSource _dataSource;

    public GetUserByIdQueryHandler(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<Result<GetUserByIdResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var getUserCommand = new CommandDefinition(_getUserSql, new { request.Id }, cancellationToken: cancellationToken);
        var user = await connection.QuerySingleOrDefaultAsync<DapperUserModel>(getUserCommand);
        if (user is null)
            return Result.Fail(new NotFound($"User with id {request.Id} was not found"));

        var getUserCardsCommand = new CommandDefinition(
            _getUserCardsSql, 
            new { UserId = request.Id }, 
            cancellationToken: cancellationToken);
        var cards = await connection.QueryAsync<DapperCardModel>(getUserCardsCommand);

        var response = new GetUserByIdResponse(
            user.Id,
            user.UserName,
            user.FirstName,
            user.LastName,
            user.Patronymic,
            user.DayOfBirth,
            user.PhoneNumber,
            user.PhoneNumberConfirmed,
            user.TwoFactorEnabled,
            user.Email,
            cards.Select(c => new CardResponse(
                c.Id,
                c.Status,
                c.Number,
                c.Balance)).ToArray());

        return Result.Ok(response);
    }

    private readonly string _getUserSql =
        """
          SELECT
              u.id AS Id,
              u.user_name AS UserName,
              u.firstname as FirstName,
              u.lastname as LastName,
              u.patronymic as Patronymic,
              u.day_of_birth AS DayOfBirth,
              u.email AS Email,
              u.phone_number AS PhoneNumber,
              u.phone_number_confirmed AS PhoneNumberConfirmed,
              u.two_factor_enabled AS TwoFactorEnabled
          FROM users AS u
          WHERE u.id = @Id;
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
        public string PhoneNumber { get; init; }
        public bool PhoneNumberConfirmed { get; init; }
        public bool TwoFactorEnabled { get; init; }
    }

    private readonly string _getUserCardsSql =
        """
        SELECT
            c.id AS Id,
            c.balance AS Balance,
            c.number AS Number,
            s.title AS Status
        FROM cards c
        JOIN statuses AS s ON c.status_id = s.id
        WHERE c.user_id = @UserId
        ORDER BY c.id;
        """;

    private class DapperCardModel
    {
        public Guid Id { get; init; }
        public string Number { get; init; }
        public decimal Balance { get; init; }
        public string Status { get; init; }
    }
}