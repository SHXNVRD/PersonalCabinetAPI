using Domain.Models;

namespace Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product?> FindById(long id, TrackingType trackingType = TrackingType.NoTracking);
}