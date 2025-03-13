using CSharpFunctionalExtensions;
using Domain.Helpers;
using Microsoft.Net.Http.Headers;
using Result = FluentResults.Result;

namespace Domain.Shared.ValueObjects;

public class CardPinHash : ValueObject
{
    public string Value { get; }

    private CardPinHash(string value) => Value = value;

    public static async Task<FluentResults.Result<CardPinHash>> Create(string pin)
    {
        if (string.IsNullOrWhiteSpace(pin)) 
            return Result.Fail($"{nameof(pin)} cannot be empty");
                        
        var trimmedPin = pin.Trim().Replace(" ", "");
        
        if (trimmedPin.Length != 4)
            return Result.Fail("Card PIN must represent a four-digit number");
        if (!trimmedPin.All(char.IsDigit))
            return Result.Fail("Card PIN must be a number");
        
        var hash = await Hasher.ComputeSha256HashAsync(trimmedPin);

        return new CardPinHash(hash.ToUpperInvariant());
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}