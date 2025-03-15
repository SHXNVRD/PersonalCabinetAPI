using Domain.Shared.Errors.Base;

namespace Domain.Shared.Errors;

public class Forbidden : DomainError
{
    public Forbidden(string message) 
        : base(message)
    { }
}