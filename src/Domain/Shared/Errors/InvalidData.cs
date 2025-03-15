using Domain.Shared.Errors.Base;

namespace Domain.Shared.Errors;

public class InvalidData : DomainError
{
    public InvalidData(string message)
        : base(message)
    { }
}