using Domain.Shared.Errors.Base;

namespace Domain.Shared.Errors;

public class ForbiddenError : DomainError
{
    public ForbiddenError(string message, ErrorCode code) 
        : base(message, code)
    { }
}