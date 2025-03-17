using Domain.Aggregates.ProductAggregate;

namespace Domain.Tests.ProductAggregate;

public class CategoryTests
{
    [Fact]
    public void Create_SuccessCase_ReturnSuccess()
    {
        var title = "Category";
        var result = Category.Create(title);
        
        Assert.True(result.IsSuccess);
        Assert.Equal(title, result.Value.Title);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_TitleIsNullOrEmpty_ReturnFail(string title)
    {
        var result = Category.Create(title);
        
        Assert.True(result.IsFailed);
    }
}