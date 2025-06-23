using System.Collections.Concurrent;
using Dapper;
using Domain.Shared;
using JsonNet.ContractResolvers;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using Quartz;

namespace Infrastructure.Data.Outbox;

[DisallowConcurrentExecution]
public class OutboxBackgroundJob(
    NpgsqlDataSource dataSource,
    IPublisher publisher,
    ILogger<OutboxBackgroundJob> logger)
    : IJob
{
    private readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All,
        ContractResolver = new PrivateSetterContractResolver()
    };

    public async Task Execute(IJobExecutionContext jobExecutionContext)
    {
        await using var connection = await dataSource.OpenConnectionAsync();
        var outboxEvents = (await connection.QueryAsync<OutboxEvent>(GetEventsSql)).AsList();

        if (outboxEvents.Count > 0)
        {
            var updateQueue = new ConcurrentQueue<Guid>();

            var domainEvents = outboxEvents
                .Select(ev => JsonConvert.DeserializeObject<DomainEvent>(ev.Content, _jsonSerializerSettings))
                .OfType<DomainEvent>()
                .AsList();

            var publishTasks = domainEvents
                .Select(domainEvent => PublishToMediatr(domainEvent, updateQueue, jobExecutionContext.CancellationToken))
                .ToList();

            await Task.WhenAll(publishTasks);

            var updateList = updateQueue.ToList();
            var paramNames = string.Join(",", updateList.Select((_, i) => $"(@EventId{i}, @CompletedAt{i})"));
            var formattedSql = string.Format(CompleteEventsSql, paramNames);

            var completedAt = DateTime.UtcNow;
            var parameters = new DynamicParameters();
            for (var i = 0; i < updateList.Count; i++)
            {
                parameters.Add($"EventId{i}", updateList[i]);
                parameters.Add($"CompletedAt{i}", completedAt);
            }

            await using var transaction = await connection.BeginTransactionAsync();
            await connection.ExecuteAsync(formattedSql, parameters, transaction);
            await transaction.CommitAsync();
        }

        return;

        async Task PublishToMediatr(
            DomainEvent domainEvent,
            ConcurrentQueue<Guid> updateQueue,
            CancellationToken cancellationToken)
        {
            try
            {
                await publisher.Publish(domainEvent, cancellationToken);
                updateQueue.Enqueue(domainEvent.Id);
            }
            catch (Exception e)
            {
                logger.LogError("Failed of processing outbox events and save update, exception: {e}", e);
            }
        }
    }

    private const string GetEventsSql =
        """
        SELECT event_id AS EventId, 
               type AS Type, 
               content AS Content, 
               created_at AS CreatedAt, 
               completed_at AS CompletedAt
        FROM outboxes
        WHERE completed_at IS NULL
        ORDER BY created_at
        LIMIT 50
        FOR UPDATE SKIP LOCKED 
        """;

    private const string CompleteEventsSql =
        """
        UPDATE outboxes
        SET completed_at = new.completed_at
        FROM (VALUES 
            {0}) AS new(event_id, completed_at)
        WHERE outboxes.event_id = new.event_id::uuid
        """;
}