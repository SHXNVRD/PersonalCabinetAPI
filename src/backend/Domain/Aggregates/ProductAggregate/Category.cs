using Domain.Aggregates.Base;
using Domain.Shared.Errors;
using FluentResults;

namespace Domain.Aggregates.ProductAggregate;

public sealed class Category : Identity<int>
{
    public static readonly Category Fuel = new(1, "Топливо");
    public static readonly Category Oil = new(2, "Масло");
    
    public string Title { get; private set; } = null!;

    private Category()
    { }

    private Category(int id, string title)
        : this()
    {
        Id = id;
        Title = title;
    }

    public static IEnumerable<Category> All() 
        => [Fuel, Oil];

    public static Result<Category> FromId(int id)
    {
        var category = All().SingleOrDefault(c => c.Id == id);
        if (category is null)
            return Errors.Conflict.NotFound("Specified category not found");

        return category;
    }
    
    public static bool operator ==(Category? a, Category? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Id == b.Id;
    }

    public static bool operator !=(Category a, Category b)
    {
        return !(a == b);
    }
}