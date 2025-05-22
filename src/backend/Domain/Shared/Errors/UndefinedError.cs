using Domain.Shared.Errors.Base;

namespace Domain.Shared.Errors;

public class UndefinedError : DomainError
{
    public UndefinedError(string message) 
        : base(message, ErrorCode.Undefined)
    { }
}