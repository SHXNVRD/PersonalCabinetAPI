using Domain.Aggregates.Base;
using Domain.Shared.Errors;
using Domain.Shared.ValueObjects;
using Result = FluentResults.Result;

namespace Domain.Aggregates.ProductAggregate;

public sealed class Product : Aggregate<long>
{
    public Category Category { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Quantity Quantity { get; private set; } = null!;
    private readonly List<ProductPriceHistory> _productPriceHistories = [];
    public IReadOnlyList<ProductPriceHistory> ProductPriceHistories => _productPriceHistories.AsReadOnly();
    public decimal Price => ProductPriceHistories
        .OrderByDescending(pph => pph.CreatedAt)
        .Select(pph => pph.Price)
        .First();
    
    private Product()
    { }

    private Product(
        string title,
        string description,
        Quantity quantity,
        Category category,
        ProductPriceHistory price) : this()
    {
        Title = title;
        Description = description;
        Quantity = quantity;
        Category = category;
        _productPriceHistories.Add(price);
    }

    public static FluentResults.Result<Product> Create(string title, string description, decimal price, Quantity quantity, Category category)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(title)} cannot be empty"));
        if (string.IsNullOrWhiteSpace(description))
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(description)} cannot be empty"));
        if (quantity is null)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(quantity)} cannot be null"));
        if (category is null)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(category)} cannot be null"));

        var priceResult = ProductPriceHistory.Create(price);
        if (priceResult.IsFailed)
            return Result.Fail(priceResult.Errors);

        return new Product(title.Trim(), description.Trim(), quantity, category, priceResult.Value);
    }

    public Result Remove(Quantity quantity)
    {
        var newQuantityResult = Quantity.Subtract(quantity);
        if (newQuantityResult.IsFailed)
            return Result.Fail(newQuantityResult.Errors);

        Quantity = newQuantityResult.Value;

        return Result.Ok();
    }
    
    public Result Add(Quantity quantity)
    {
        var newQuantityResult = Quantity.Add(quantity);
        if (newQuantityResult.IsFailed)
            return Result.Fail(newQuantityResult.Errors);

        Quantity = newQuantityResult.Value;

        return Result.Ok();
    }

    public Result ChangePrice(decimal price)
    {
        var newPriceResult = ProductPriceHistory.Create(price);
        if (newPriceResult.IsFailed)
            return Result.Fail(newPriceResult.Errors);
        
        _productPriceHistories.Add(newPriceResult.Value);

        return Result.Ok();
    }

    public Result ChangeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(title)} cannot be empty"));

        Title = title.Trim();
        
        return Result.Ok();
    }
    
    public Result ChangeDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(description)} cannot be empty"));

        Title = description.Trim();
        
        return Result.Ok();
    }
    
    public Result ChangeQuantity(Quantity quantity)
    {
        if (quantity is null)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(quantity)} cannot ne null"));

        Quantity = quantity;
        
        return Result.Ok();
    }

    public Result ChangeCategory(Category category)
    {
        if (category is null)
            return Result.Fail(Errors.InvalidData.ValidationFailed($"{nameof(category)} cannot be null"));

        Category = category;

        return Result.Ok();
    }
}