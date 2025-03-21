using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Repositories;
using Domain.Aggregates.CardAggregate;
using Domain.Shared.ValueObjects;
using Infrastructure.Extensions;

namespace Infrastructure.Data.Repositories;

public class CardRepository : ICardRepository
{
    private readonly AppDbContext _context;

    public CardRepository(AppDbContext context)
    {
            _context = context;
        }

    public Task<Card?> FindByIdAsync(Guid id, TrackingType trackingType = TrackingType.NoTracking)
    {
            return _context.Cards
                .SetTracking(trackingType)
                .SingleOrDefaultAsync(c => c.Id == id);
        }
        
    public Task<Card?> FindByNumberAsync(CardNumber number, TrackingType trackingType = TrackingType.NoTracking)
    {
            return _context.Cards
                .SetTracking(trackingType)
                .Include(c => c.Status)
                .SingleOrDefaultAsync(c => c.Number == number);
        }
        
    public Task<Card?> FindByNumberWithPurchasesAndRefundsAsync(CardNumber number, TrackingType trackingType = TrackingType.NoTracking)
    {
            return _context.Cards
                .SetTracking(trackingType)
                .Include(c => c.Refunds).AsSplitQuery()
                .Include(c => c.Purchases)
                .ThenInclude(p => p.PurchaseItems)
                .ThenInclude(pi => pi.Product)
                .SingleOrDefaultAsync(c => c.Number == number);
        }
        
    public Task<Card?> FindByUserIdAsync(Guid id, TrackingType trackingType = TrackingType.NoTracking)
    {
            return _context.Cards
                .SetTracking(trackingType)
                .FirstOrDefaultAsync(c => c.UserId == id);
        }

    public async Task<bool> ActivateAsync(Guid userId, CardNumber number, CardPinHash pinHash)
    {
            var activatedCards = await _context.Cards
                .Where(c => c.Number == number && c.PinHash == pinHash)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.UserId, userId)
                    .SetProperty(c => c.ActivatedAt, DateTime.UtcNow)
                    .SetProperty(c => c.Status, Status.Activated));
            
            return activatedCards != 0;
        }

    public async Task<bool> DeactivateAsync(CardNumber number)
    {
            var deactivatedCards = await _context.Cards
                .Where(c => c.Number == number)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.Status, Status.Blocked));
                    
            return deactivatedCards != 0;
        }
}