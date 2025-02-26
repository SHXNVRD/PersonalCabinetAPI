using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Models;
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

    public async Task<User?> GetByPhoneNumber(string phone, TrackingType trackingType) =>
        await _context.Users
            .SetTracking(trackingType)
            .FirstOrDefaultAsync(u => u.PhoneNumber == phone);
}