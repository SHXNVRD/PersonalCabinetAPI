using Domain.Aggregates.CardAggregate;

namespace Domain.Shared.Errors;

public static class Errors
{
    public static class Conflict
    {
        public static ConflictError CannotSetCardStatus(Status currentStatus, string message = "Status cannot be settled")
            => (ConflictError)new ConflictError(message, ErrorCode.CannotSetCardStatus).WithMetadata(ConflictError.CurrentCardStatusMetadataKey, currentStatus);

        public static ConflictError WrongCardPin(string message = "Wrong card pin")
            => new ConflictError(message, ErrorCode.WrongCardPin);

        public static ConflictError FuelIsForbidden(string message = "Fuel is forbidden for this card")
            => new ConflictError(message, ErrorCode.FuelForbidden);

        public static ConflictError InsufficientCardFunds(string message = "Insufficient card funds")
            => new ConflictError(message, ErrorCode.InsufficientCardFunds);

        public static ConflictError InvalidCurrentCardStatus(Status currentStatus, string message = "Unable to perform an action with the current card status")
            => (ConflictError)new ConflictError(message, ErrorCode.InvalidStatus).WithMetadata(ConflictError.CurrentCardStatusMetadataKey, currentStatus);

        public static ConflictError FailedReturnFunds(string message = "Cannot refund purchase")
            => new ConflictError(message, ErrorCode.CannotRefundPurchase);

        public static ConflictError Duplicate(string message = "Uniqueness violations")
            => new ConflictError(message, ErrorCode.Duplication);

        public static ConflictError LowAvailableQuantity(string message = "Requested quantity is greater than the quantity available")
            => new ConflictError(message, ErrorCode.UnavailableQuantity);

        public static ConflictError Mismatch(string message = "Transmitted value does not match the current value")
            => new ConflictError(message, ErrorCode.Mismatch);

        public static ConflictError AlreadyHasValue(string message = "Value already set")
            => new ConflictError(message, ErrorCode.AlreadyHasValue);
        
        public static ConflictError NotFound(string message = "Entity not found")
            => new ConflictError(message, ErrorCode.NotFound);
    }

    public static class InvalidData
    {
        public static InvalidDataError ValidationFailed(string message = "Validation failed")
            => new InvalidDataError(message);
    }

    public static class Forbidden
    {
        public static ForbiddenError UserNotInRole(string message = "User not in role")
            => new ForbiddenError(message, ErrorCode.UserNotInRole);
    }

    public static class Unauthorized
    {
        public static UnauthorizedError WrongCredentials(string message = "Wrong credentials")
            => new UnauthorizedError(message, ErrorCode.WrongCredentials);

        public static UnauthorizedError UnconfirmedEmail(string message = "Unconfirmed email")
            => new UnauthorizedError(message, ErrorCode.UnconfirmedEmail);

        public static UnauthorizedError FailedToRefreshToken(string message = "Failed to refresh token")
            => new UnauthorizedError(message, ErrorCode.FailedToRefreshToken);
    }

    public static class Undefined
    {
        public static UndefinedError UndefinedError(string message = "Undefined error")
            => new UndefinedError(message);
    }
}