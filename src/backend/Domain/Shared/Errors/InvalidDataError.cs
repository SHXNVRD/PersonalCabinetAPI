using Domain.Shared.Errors.Base;

namespace Domain.Shared.Errors;

public class InvalidDataError : DomainError
{
    public InvalidDataError(string message)
        : base(message, ErrorCode.ValidationFailed)
    { }
}