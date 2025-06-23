using Domain.Aggregates.ProductAggregate;
using Domain.Shared.ValueObjects;

namespace Domain.Tests.ProductAggregate;

public class ProductTests
{
    private readonly Category _category = Category.Fuel;
    private readonly Quantity _one = Quantity.Create(1m).Value;
    private readonly string _productTitle = "Product";
    private readonly string _productDescription = "Description";
    private readonly decimal _productPrice = 1m;

    [Fact]
    public void Create_SuccessCase_ReturnsSuccess()
    {
        var result = Product.Create(_productTitle, _productDescription, _productPrice, _one, _category);

        var product = result.ValueOrDefault;
        Assert.True(result.IsSuccess);
        Assert.Equal(_productTitle, product.Title);
        Assert.Equal(_productDescription, product.Description);
        Assert.Equal(_one, product.Quantity);
        Assert.Equal(_category, product.Category);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_TitleIsNullOrEmpty_ReturnsFail(string title)
    {
        var result = Product.Create(title, _productDescription, _productPrice, _one, _category);

        Assert.True(result.IsFailed);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_DescriptionIsNullOrEmpty_ReturnsFail(string description)
    {
        var result = Product.Create(_productTitle, description, _productPrice, _one, _category);

        Assert.True(result.IsFailed);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_PriceLessThenZero_ReturnsFail(decimal price)
    {
        var result = Product.Create(_productTitle, _productDescription, price, _one, _category);

        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public void Create_QuantityIsNull_ReturnsFail()
    {
        var result = Product.Create(_productTitle, _productDescription, _productPrice, null, _category);

        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public void Create_CategoryIsNull_ReturnsFail()
    {
        var result = Product.Create(_productTitle, _productDescription, _productPrice, _one, null);

        Assert.True(result.IsFailed);
    }

    [Fact]
    public void Add_SuccessCase_ReturnsSuccess()
    {
        var product = Product.Create(_productTitle, _productDescription, _productPrice, _one, _category).Value;

        var result = product.Add(_one);

        Assert.True(result.IsSuccess);
        Assert.Equal(Quantity.Create(2m).Value, product.Quantity);
    }
    
    [Fact]
    public void Add_QuantityIsNull_ReturnsFail()
    {
        var product = Product.Create(_productTitle, _productDescription, _productPrice, _one, _category).Value;

        var result = product.Add(null);
        
        Assert.True(result.IsFailed);
    }
    
    [Fact]
    public void Remove_SuccessCase_ReturnsSuccess()
    {
        var product = Product.Create(_productTitle, _productDescription, _productPrice, _one, _category).Value;

        var result = product.Remove(_one);

        Assert.True(result.IsSuccess);
        Assert.Equal(Quantity.Create(0m).Value, product.Quantity);
    }
    
    [Fact]
    public void Remove_QuantityIsNull_ReturnsFail()
    {
        var product = Product.Create(_productTitle, _productDescription, _productPrice, _one, _category).Value;

        var result = product.Remove(null);
        
        Assert.True(result.IsFailed);
    }
}