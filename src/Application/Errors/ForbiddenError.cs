using Application.Errors.Base;
using Domain;

namespace Application.Errors;

public class ForbiddenError : ApplicationError
{
    protected ForbiddenError(ErrorType errorType, string message)
        : base(errorType, message)
    { }

    public ForbiddenError(string message)
    :this(ErrorType.Forbidden, message)
    { }
}