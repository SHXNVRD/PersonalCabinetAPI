using Application.DTOs.Emails;
using Application.Interfaces;
using Application.Interfaces.Email;
using Application.Services;
using Dapper;
using Domain.Aggregates.CardAggregate.DomainEvents;
using Domain.Shared.Exceptions;
using MediatR;
using Npgsql;

namespace Application.Cards.DomainEventHandlers;

public class CardBlockedDomainEventHandler : INotificationHandler<CardBlockedDomainEvent>
{
    private readonly IEmailService _emailService;
    private readonly NpgsqlDataSource _dataSource;

    public CardBlockedDomainEventHandler(IEmailService emailService, NpgsqlDataSource dataSource)
    {
        _emailService = emailService;
        _dataSource = dataSource;
    }

    public async Task Handle(CardBlockedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);

        var getUserCommand = new CommandDefinition(_sql, new { Id = domainEvent.UserId }, cancellationToken: cancellationToken);
        var user = await connection.QuerySingleOrDefaultAsync<DapperUserModel>(getUserCommand);
        if (user is null)
            throw new DataConsistencyViolationException($"{nameof(CardBlockedDomainEvent)} contains {nameof(domainEvent.UserId)} for non-exist user");

        var message = new EmailMessage("Блокировка топливной карты", [user.Email]);
        await _emailService.SendCardBlockedAsync(message, user.Name, domainEvent.CardNumber, 3, cancellationToken);
    }

    private readonly string _sql =
        """
        SELECT 
            user_name AS Name,
            email AS Email
        FROM users
        WHERE id = @Id
        """;

    private class DapperUserModel
    {
        public string Name { get; init; }
        public string Email { get; init; }
    }
}