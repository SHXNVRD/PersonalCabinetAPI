using Domain.Shared.Errors.Base;

namespace Domain.Shared.Errors;

public class UnauthorizedError : DomainError
{
    public UnauthorizedError(string message, ErrorCode code)
        : base(message, code)
    { }
}