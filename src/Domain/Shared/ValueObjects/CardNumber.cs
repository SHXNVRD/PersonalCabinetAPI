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
            return Result.Fail("Card number cannot be empty");

        var number = value.Trim().Replace(" ", "");

        if (number.Length != 12)
            return Result.Fail("Card number must be 12 digits");
        if (!number.All(char.IsDigit))
            return Result.Fail("Card number must be a number");

        return new CardNumber(number);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}