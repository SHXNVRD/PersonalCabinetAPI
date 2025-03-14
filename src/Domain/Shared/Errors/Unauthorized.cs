using FluentResults;

namespace Domain.Shared.Errors;

public class Unauthorized : Error
{
    public Unauthorized(string message)
        : base(message)
    { }
}