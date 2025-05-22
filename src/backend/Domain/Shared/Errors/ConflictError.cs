using Domain.Shared.Errors.Base;
namespace Domain.Shared.Errors;

public class ConflictError : DomainError
{
    public ConflictError(string message, ErrorCode code) 
        : base(message, code)
    { }
}