using Dapper;
using FluentResults;
using MediatR;
using Npgsql;

namespace Application.Cards.Queries;

public class GetOperationsByIdQueryHandler : IRequestHandler<GetOperationsByIdQuery, Result<GetOperationsByIdResponse>>
{
    private readonly NpgsqlDataSource _dataSource;

    public GetOperationsByIdQueryHandler(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<Result<GetOperationsByIdResponse>> Handle(GetOperationsByIdQuery request, CancellationToken cancellationToken)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        await using var multi = await connection.QueryMultipleAsync(
            _sql,
            new
            {
                CardId = request.CardId,
                OffSet = request.Page-- * request.PageSize,
                Limit = request.PageSize
            });

        var operations = (await multi.ReadAsync<CardOperation>()).ToList();
        return new GetOperationsByIdResponse(operations);
    }

    private readonly string _sql =
        """
        WITH operations AS (
            SELECT
                p.id AS Id,
                true AS IsPurchase,
                p.created_at AS CreatedAt,
                COALESCE(SUM(pi.quantity * pi.product_price_at_purchase), 0) AS Total
            FROM purchases p
            JOIN purchase_items pi on p.id = pi.purchase_id 
            WHERE card_id = @CardId
            GROUP BY p.id
        
            UNION ALL
        
            SELECT
                id AS Id,
                false AS IsPurchase,
                created_at AS CreatedAt,
                total AS Total
            FROM refunds
            WHERE card_id = @CardId)
        
            SELECT
                Id,
                IsPurchase,
                CreatedAt,
                Total
            FROM operations
            ORDER BY CreatedAt DESC
            OFFSET @Offset
            LIMIT @Limit;
        """;
}