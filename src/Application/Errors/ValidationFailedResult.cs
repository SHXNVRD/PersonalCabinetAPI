using Application.Errors.Base;
using Domain;
using Domain.Shared;

namespace Application.Errors;

public class ValidationFailedResult : ApplicationError
{
    protected ValidationFailedResult(ErrorType errorType, string message)
        : base(errorType, message)
    { }

    public ValidationFailedResult(string message)
    :this(ErrorType.ValidationFailed, message)
    { }
}