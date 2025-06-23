using Domain.Aggregates.CardAggregate;
using Domain.Shared.Errors;

namespace Domain.Extensions;

public static class ConflictErrorExtensions
{
    public static bool IsHasCardStatus(this ConflictError error, Status status)
    {
        if (!error.Metadata.TryGetValue(ConflictError.CurrentCardStatusMetadataKey, out var currentCardStatus))
            return false;

        if (currentCardStatus is Status currentStatus)
            return currentStatus == status;

        return false;
    }
}