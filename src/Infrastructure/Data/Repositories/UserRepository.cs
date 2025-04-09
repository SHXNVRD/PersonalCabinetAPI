using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Aggregates.CardAggregate;
using Domain.Aggregates.UserAggregate;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> FindByPhoneNumberAsync(string phone, TrackingType trackingType = TrackingType.NoTracking) =>
        await _context.Users
            .SetTracking(trackingType)
            .FirstOrDefaultAsync(u => u.PhoneNumber == phone);

    public async Task<User?> FindByIdAsync(Guid id, TrackingType trackingType = TrackingType.NoTracking)
        => await _context.Users
            .SetTracking(trackingType)
            .Include(u => u.Cards)
            .ThenInclude(c => c.Status)
            .SingleOrDefaultAsync(u => u.Id == id);
}