using Domain.Models;

namespace Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product?> FindById(long id, TrackingType trackingType = TrackingType.NoTracking);
    Task<decimal> GetPriceAtDateAsync(long productId, DateTime purchaseDate);
}