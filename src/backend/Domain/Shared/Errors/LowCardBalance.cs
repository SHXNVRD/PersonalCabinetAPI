using Domain.Shared.Errors.Base;

namespace Domain.Shared.Errors;

public class LowCardBalance : DomainError
{
    public LowCardBalance(string message) 
        : base(message)
    { }
}