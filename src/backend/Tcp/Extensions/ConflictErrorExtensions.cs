using Domain.Aggregates.CardAggregate;
using Domain.Extensions;
using Domain.Shared.Errors;
using Domain.Shared.Errors.Base;

namespace Tcp.Extensions;

public static class ConflictErrorExtensions
{
    public static string? GetTerminalErrorCode(this ConflictError error)
    {
        if (!error.Metadata.TryGetValue(DomainError.ErrorCodeMetadataKey, out var domainErrorCode))
            return null;
        
        if (domainErrorCode is ErrorCode code)
        {
            return code switch
            {
                ErrorCode.FuelForbidden => ErrorCodes.FuelForbidden,
                ErrorCode.WrongCardPin => ErrorCodes.WrongCardPin,
                ErrorCode.InvalidStatus when error.IsHasCardStatus(Status.Blocked) => ErrorCodes.CardBlocked,
                _ => null
                //TODO Подумать над функциональность закрытия карты на обслуживание
                //ErrorCode.InvalidStatus when WhenShouldSetErrorCodeToCardMaintenance => ErrorCodes.CardMaintenance
            };
        }

        return null;
    }
}