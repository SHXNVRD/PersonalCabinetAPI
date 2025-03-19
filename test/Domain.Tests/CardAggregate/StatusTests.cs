using Domain.Aggregates.CardAggregate;

namespace Domain.Tests.CardAggregate;

public class StatusTests
{
    [Fact]
    public void CanChangeTo_FromUnusedToUnused_ReturnsFalse()
    {
        var result = Status.Unused.CanChangeTo(Status.Unused);
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanChangeTo_FromUnusedToActivated_ReturnsTrue()
    {
        var result = Status.Unused.CanChangeTo(Status.Activated);
        
        Assert.True(result);
    }
    
    [Fact]
    public void CanChangeTo_FromUnusedToBlocked_ReturnsFalse()
    {
        var result = Status.Unused.CanChangeTo(Status.Blocked);
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanChangeTo_FromActivatedToUnUsed_ReturnsFalse()
    {
        var result = Status.Activated.CanChangeTo(Status.Unused);
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanChangeTo_FromActivatedToActivated_ReturnsFalse()
    {
        var result = Status.Activated.CanChangeTo(Status.Activated);
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanChangeTo_FromActivatedToBlocked_ReturnsTrue()
    {
        var result = Status.Activated.CanChangeTo(Status.Blocked);
        
        Assert.True(result);
    }
    
    [Fact]
    public void CanChangeTo_FromBlockedToUnUsed_ReturnsFalse()
    {
        var result = Status.Blocked.CanChangeTo(Status.Unused);
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanChangeTo_FromBlockedToActivated_ReturnsFalse()
    {
        var result = Status.Blocked.CanChangeTo(Status.Activated);
        
        Assert.False(result);
    }
    
    [Fact]
    public void CanChangeTo_FromBlockedToBlocked_ReturnsFalse()
    {
        var result = Status.Blocked.CanChangeTo(Status.Blocked);
        
        Assert.False(result);
    }

    [Fact]
    public void CanChangeTo_StatusIsNull_ThrowArgumentNullException()
    {
        void Act() => Status.Blocked.CanChangeTo(null);
        
        Assert.Throws<ArgumentNullException>(Act);
    }
}