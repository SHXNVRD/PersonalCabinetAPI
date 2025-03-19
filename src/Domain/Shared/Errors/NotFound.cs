using Domain.Shared.Errors.Base;

namespace Domain.Shared.Errors;

public class NotFound : DomainError
{
    public NotFound(string message)
        : base(message)
    { }
}