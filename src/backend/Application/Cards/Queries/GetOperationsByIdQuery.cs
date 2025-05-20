using FluentResults;
using MediatR;

namespace Application.Cards.Queries;

public class GetOperationsByIdQuery : IRequest<Result<GetOperationsByIdResponse>>
{
    public Guid CardId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class GetOperationsByIdResponse
{
    private readonly List<CardOperation> _operations;
    public IReadOnlyList<CardOperation> Operations => _operations.AsReadOnly(); 
        
    public GetOperationsByIdResponse(List<CardOperation> operations)
    {
        _operations = operations;
    }
}

public class CardOperation
{
    public Guid Id { get; set; }
    public bool IsPurchase { get; set; }
    public decimal Total { get; set; }
    public DateTime CreatedAt { get; set; }
}
