using Domain.Aggregates.Base;
using Domain.Shared.Errors;
using Domain.Shared.ValueObjects;
using FluentResults;

namespace Domain.Aggregates.ProductAggregate;

public sealed class Product : Identity<long>
{
    public Category Category { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Quantity Quantity { get; private set; } = null!;
    private List<ProductPriceHistory> _productPriceHistories = [];
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

    public static Result<Product> Create(string title, string description, decimal price, Quantity quantity, Category category)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Result.Fail(new InvalidData($"{nameof(title)} cannot be empty"));
        if (string.IsNullOrWhiteSpace(description))
            return Result.Fail(new InvalidData($"{nameof(description)} cannot be empty"));
        if (quantity is null)
            return Result.Fail(new InvalidData($"{nameof(quantity)} cannot be null"));
        if (category is null)
            return Result.Fail(new InvalidData($"{nameof(category)} cannot be null"));

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
}