using Application.Errors.Base;
using Domain;
using Domain.Shared;

namespace Application.Errors;

public class ConflictError : ApplicationError
{
    protected ConflictError(ErrorType errorType, string message)
        : base(errorType, message)
    { }
    
    public ConflictError(string message)
        :this(ErrorType.Conflict, message)
    { }
}