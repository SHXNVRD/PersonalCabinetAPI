using CSharpFunctionalExtensions;
using Result = FluentResults.Result;

namespace Domain.Shared.ValueObjects;

public class Quantity : ValueObject
{
    public decimal Value { get; }

    private Quantity(decimal value) => Value = value;

    public static FluentResults.Result<Quantity> Create(decimal value)
    {
        if (value < 0)
            return Result.Fail(Errors.Errors.InvalidData.ValidationFailed("Quantity must be greater or equals zero"));

        return new Quantity(value);
    }

    public FluentResults.Result<Quantity> Add(Quantity quantity)
    {
        if (quantity is null)
            return Result.Fail(Errors.Errors.InvalidData.ValidationFailed($"{nameof(quantity)} cannot ne null"));
        
        var newValue = Value + quantity.Value;
        return Create(newValue);
    }

    public FluentResults.Result<Quantity> Subtract(Quantity quantity)
    {
        if (quantity is null)
            return Result.Fail(Errors.Errors.InvalidData.ValidationFailed($"{nameof(quantity)} cannot ne null"));
        
        var newValue = Value - quantity.Value;
        return Create(newValue);
    }

    public static bool operator > (Quantity a, Quantity b)
        => a.Value > b.Value;

    public static bool operator <(Quantity a, Quantity b)
        => a.Value < b.Value;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}