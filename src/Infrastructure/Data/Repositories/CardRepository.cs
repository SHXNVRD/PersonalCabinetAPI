using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Repositories;
using Domain.Aggregates.CardAggregate;
using Infrastructure.Extensions;

namespace Infrastructure.Data.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly AppDbContext _context;

        public CardRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<Card?> FindByIdAsync(long id, TrackingType trackingType = TrackingType.NoTracking)
        {
            return _context.Cards
                .SetTracking(trackingType)
                .SingleOrDefaultAsync(c => c.Id == id);
        }
        
        public Task<Card?> FindByNumberAsync(string number, TrackingType trackingType = TrackingType.NoTracking)
        {
            return _context.Cards
                .SetTracking(trackingType)
                .SingleOrDefaultAsync(c => c.Number == number);
        }

        public Task<Card?> FindByUserIdAsync(long id, TrackingType trackingType = TrackingType.NoTracking)
        {
            return _context.Cards
                .SetTracking(trackingType)
                .FirstOrDefaultAsync(c => c.UserId == id);
        }

        public async Task<bool> ActivateAsync(long userId, string number, string pinCodeHash)
        {
            int activatedCards = await _context.Cards
                .Where(c => c.Number == number && c.PinCodeHash == pinCodeHash)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.UserId, userId)
                    .SetProperty(c => c.ActivatedAt, DateTime.UtcNow)
                    .SetProperty(c => c.IsActivated, true));
            
            return activatedCards != 0;
        }

        public async Task<bool> DeactivateAsync(string number)
        {
            int deactivatedCards = await _context.Cards
                .Where(c => c.Number == number)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.IsActivated, false));
                    
            return deactivatedCards != 0;
        }
    }
}