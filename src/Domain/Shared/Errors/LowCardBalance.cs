namespace Domain.Shared.Errors;

public class LowCardBalance : Conflict
{
    public LowCardBalance(string message) 
        : base(message)
    { }
}