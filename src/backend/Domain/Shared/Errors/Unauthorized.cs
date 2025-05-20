using Domain.Shared.Errors.Base;

namespace Domain.Shared.Errors;

public class Unauthorized : DomainError
{
    public Unauthorized(string message)
        : base(message)
    { }
}