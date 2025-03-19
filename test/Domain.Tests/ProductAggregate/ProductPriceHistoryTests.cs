using Domain.Aggregates.ProductAggregate;

namespace Domain.Tests.ProductAggregate;

public class ProductPriceHistoryTests
{
    [Fact]
    public void Create_SuccessCase_ReturnsSuccess()
    {
        var result = ProductPriceHistory.Create(1m);
        
        Assert.True(result.IsSuccess);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_PriceLessThenZero_ReturnsFail(decimal price)
    {
        var result = ProductPriceHistory.Create(price);
        
        Assert.True(result.IsFailed);
    }
}