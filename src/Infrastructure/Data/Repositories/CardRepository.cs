using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces.Repositories;

namespace Infrastructure.Data.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly AppDbContext _context;

        public CardRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<Card?> FindByIdAsync(long id)
        {
            return _context.Cards
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Id == id);
        }
        
        public Task<Card?> FindByNumberAsync(int number)
        {
            return _context.Cards
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Number == number);
        }

        public async Task<bool> ActivateAsync(long userId, int number, string pinCodeHash)
        {
            int activatedCards = await _context.Cards
                .Where(c => c.Number == number && c.PinCodeHash == pinCodeHash)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.UserId, userId)
                    .SetProperty(c => c.ActivatedAt, DateTime.UtcNow)
                    .SetProperty(c => c.IsActivated, true));
            
            return activatedCards != 0;
        }

        public async Task<bool> DeactivateAsync(int number)
        {
            int deactivatedCards = await _context.Cards
                .Where(c => c.Number == number)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.IsActivated, false));
                    
            return deactivatedCards != 0;
        }

        public Task<Card?> FindByUserIdAsync(long id)
        {
            return _context.Cards
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == id);
        }
    }
}