using FluentResults;

namespace Domain.Shared.Errors.Base;

public class DomainError : Error
{
    protected DomainError(string message) 
        : base(message)
    { }
}