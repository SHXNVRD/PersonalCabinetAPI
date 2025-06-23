using Domain.Aggregates.UserAggregate;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> FindByPhoneNumberAsync(string phone, TrackingType trackingType = TrackingType.NoTracking);
    Task<User?> FindByIdAsync(Guid id, TrackingType trackingType = TrackingType.NoTracking);
}