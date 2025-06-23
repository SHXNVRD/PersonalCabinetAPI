using Domain.Aggregates.ProductAggregate;

namespace Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product?> FindById(long id, TrackingType trackingType = TrackingType.NoTracking);
    Task AddAsync(Product product);
    Task<bool> RemoveAsync(long id);
    void Update(Product product);
}