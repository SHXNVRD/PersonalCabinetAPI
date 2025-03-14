using FluentResults;

namespace Domain.Shared.Errors;

public class Forbidden : Error
{
    public Forbidden(string message) 
        : base(message)
    { }
}