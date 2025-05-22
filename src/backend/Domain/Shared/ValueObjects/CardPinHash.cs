using CSharpFunctionalExtensions;
using Domain.Helpers;
using Domain.Shared.Errors;
using Microsoft.Net.Http.Headers;
using Result = FluentResults.Result;

namespace Domain.Shared.ValueObjects;

public class CardPinHash : ValueObject
{
    public string Value { get; }

    private CardPinHash(string value) => Value = value;

    public static FluentResults.Result<CardPinHash> Create(string pin)
    {
        if (string.IsNullOrWhiteSpace(pin)) 
            return Result.Fail(Errors.Errors.InvalidData.ValidationFailed($"{nameof(pin)} cannot be empty"));
                        
        var trimmedPin = pin.Trim().Replace(" ", "");
        
        if (trimmedPin.Length != 4)
            return Result.Fail(Errors.Errors.InvalidData.ValidationFailed("Card pin must be 4 digits"));
        if (!trimmedPin.All(char.IsDigit))
            return Result.Fail(Errors.Errors.InvalidData.ValidationFailed("Card pin must be a number"));
        
        var hash = Hasher.ComputeSha256Hash(trimmedPin);

        return new CardPinHash(hash);
    }
    
    public static bool operator ==(CardPinHash? a, CardPinHash? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Value == b.Value;
    }

    public static bool operator !=(CardPinHash? a, CardPinHash? b)
        => !(a == b);
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}