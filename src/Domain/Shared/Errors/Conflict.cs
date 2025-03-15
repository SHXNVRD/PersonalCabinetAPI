using Domain.Shared.Errors.Base;
namespace Domain.Shared.Errors;

public class Conflict : DomainError
{
    public Conflict(string message) 
        : base(message)
    { }
}