using FluentResults;

namespace Domain.Shared.Errors;

public class InvalidData : Error
{
    public InvalidData(string message)
        : base(message)
    { }
}