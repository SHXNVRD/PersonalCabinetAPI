using Application.Cards.Queries;
using Riok.Mapperly.Abstractions;

namespace API.Controllers.Card.DTOs;

public record GetOperationsByIdRequest(
    Guid CardId,
    int Page,
    int PageSize);

[Mapper]
public static partial class GetOperationsByIdMapper
{
    public static partial GetOperationsByIdQuery ToQuery(GetOperationsByIdRequest request);
}
