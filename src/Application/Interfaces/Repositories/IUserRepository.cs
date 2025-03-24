using Domain.Aggregates.UserAggregate;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> FindByPhoneNumber(string phone, TrackingType trackingType = TrackingType.NoTracking);
    Task<User?> FindWithCardsById(Guid id, TrackingType trackingType = TrackingType.NoTracking);
}