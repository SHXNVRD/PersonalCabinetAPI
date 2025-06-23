using Domain.Shared.Errors.Base;

namespace Domain.Shared.Errors;

public class NotFoundError : DomainError
{
    public NotFoundError(string message)
        : base(message, ErrorCode.NotFound)
    { }
}