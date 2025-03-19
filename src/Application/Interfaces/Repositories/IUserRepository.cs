using Domain.Aggregates.UserAggregate;

namespace Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByPhoneNumber(string phone, TrackingType trackingType = TrackingType.NoTracking);
}