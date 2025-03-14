using FluentResults;

namespace Domain.Shared.Errors;

public class Conflict : Error
{
    public Conflict(string message) 
        : base(message)
    { }
}