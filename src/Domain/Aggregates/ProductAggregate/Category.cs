using Domain.Aggregates.Base;
using Domain.Shared.Errors;
using FluentResults;

namespace Domain.Aggregates.ProductAggregate;

public sealed class Category : Identity<Guid>
{
    public string Title { get; private set; } = null!;

    private Category()
    { }

    private Category(string title)
        : this()
    {
        Id = Guid.NewGuid();
        Title = title;
    }

    public static Result<Category> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Fail(new InvalidData($"{nameof(title)} cannot be empty"));

        return new Category(title.Trim());
    }
}