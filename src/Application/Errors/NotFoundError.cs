using Application.Errors.Base;
using Domain;

namespace Application.Errors;

public class NotFoundError : ApplicationError
{
    protected NotFoundError(ErrorType errorType, string message)
        : base(errorType, message)
    { }

    public NotFoundError(string message)
        : this(ErrorType.NotFound, message)
    { }
}