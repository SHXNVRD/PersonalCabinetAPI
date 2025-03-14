using Domain.Aggregates.Base;
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

    internal Result Remove(Quantity quantity)
    {
        var newQuantityResult = Quantity.Subtract(quantity);
        if (newQuantityResult.IsFailed)
            return Result.Fail(newQuantityResult.Errors);

        Quantity = newQuantityResult.Value;

        return Result.Ok();
    }
    
    internal Result Add(Quantity quantity)
    {
        var newQuantityResult = Quantity.Add(quantity);
        if (newQuantityResult.IsFailed)
            return Result.Fail(newQuantityResult.Errors);

        Quantity = newQuantityResult.Value;

        return Result.Ok();
    }
}