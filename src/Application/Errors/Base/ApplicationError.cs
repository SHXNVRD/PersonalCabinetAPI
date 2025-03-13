using Domain;
using Domain.Shared;
using FluentResults;

namespace Application.Errors.Base;

public class ApplicationError : Error
{
    public ErrorType ErrorType { get; init; }

    protected ApplicationError(ErrorType errorType, string message)
        : base(message)
    {
        ErrorType = errorType;
    }
}