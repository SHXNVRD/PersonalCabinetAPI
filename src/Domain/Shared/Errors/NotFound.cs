using FluentResults;

namespace Domain.Shared.Errors;

public class NotFound : Error
{
    public NotFound(string message)
        : base(message)
    { }
}