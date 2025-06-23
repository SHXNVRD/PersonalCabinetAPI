using CSharpFunctionalExtensions;
using Result = FluentResults.Result;

namespace Domain.Shared.ValueObjects;

public class Name : ValueObject
{
    public string FirstName { get; }
    public string LastName { get; }
    public string? Patronymic { get; } 
    
    private Name(
        string firstName,
        string lastName,
        string? patronymic)
    {
        FirstName = firstName;
        LastName = lastName;
        Patronymic = patronymic;
    }

    public static FluentResults.Result<Name> Create(string firstName, string lastName, string? patronymic = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return Result.Fail(Errors.Errors.InvalidData.ValidationFailed($"{nameof(firstName)} cannot be empty"));
        if (string.IsNullOrWhiteSpace(lastName))
            return Result.Fail(Errors.Errors.InvalidData.ValidationFailed($"{nameof(lastName)} cannot be empty"));
        if (patronymic is not null && string.IsNullOrWhiteSpace(patronymic))
            return Result.Fail(Errors.Errors.InvalidData.ValidationFailed($"{nameof(patronymic)} cannot be empty"));

        return new Name(firstName, lastName, patronymic);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
        if (Patronymic is not null)
            yield return Patronymic;
    }
}