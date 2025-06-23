using CSharpFunctionalExtensions;
using Result = FluentResults.Result;

namespace Domain.Shared.ValueObjects;

public class CardNumber : ValueObject
{
    public string Value { get; }

    private CardNumber(string value) => Value = value;

    public static FluentResults.Result<CardNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Fail(Errors.Errors.InvalidData.ValidationFailed($"{nameof(value)} cannot be empty"));

        var number = value.Trim().Replace(" ", "");

        if (number.Length != 12)
            return Result.Fail(Errors.Errors.InvalidData.ValidationFailed("Card number must be 12 digits"));
        if (!number.All(char.IsDigit))
            return Result.Fail(Errors.Errors.InvalidData.ValidationFailed("Card number must be a number"));

        return new CardNumber(number);
    }

    public static bool operator ==(CardNumber? a, CardNumber? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Value == b.Value;
    }

    public static bool operator !=(CardNumber? a, CardNumber? b)
        => !(a == b);
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}