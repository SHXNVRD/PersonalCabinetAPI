namespace Domain.Shared.Exceptions;

public class DataConsistencyViolationException : Exception
{
    public DataConsistencyViolationException(string message = "Data consistency violation")
    : base(message)
    { }
}