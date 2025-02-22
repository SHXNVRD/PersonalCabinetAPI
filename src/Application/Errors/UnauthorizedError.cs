using Application.Errors.Base;
using Domain;

namespace Application.Errors;

public class UnauthorizedError : ApplicationError
{
    protected UnauthorizedError(ErrorType errorType, string message)
        : base(errorType, message)
    { }
    
    public UnauthorizedError(string message)
        :this(ErrorType.Unauthorized, message)
    { }
}