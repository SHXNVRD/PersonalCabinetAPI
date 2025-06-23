namespace Domain.Shared.Errors;

public enum ErrorCode
{
    CannotSetCardStatus,
    InvalidStatus,
    WrongCardPin,
    FuelForbidden,
    InsufficientCardFunds,
    CannotRefundPurchase,
    Duplication,
    UnavailableQuantity,
    Mismatch,
    AlreadyHasValue,
    ValidationFailed,
    NotFound,
    UserNotInRole,
    Undefined,
    WrongCredentials,
    UnconfirmedEmail,
    FailedToRefreshToken
}