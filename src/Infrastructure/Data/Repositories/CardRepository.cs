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

        public async Task<Card?> FindByIdAsync(long id)
        {
            return await _context.Cards
                .AsNoTracking()
                .Include(c => c.BonusSystem)
                .SingleOrDefaultAsync(c => c.Id == id);
        }
        
        public async Task<Card?> FindByNumberAsync(int number)
        {
            return await _context.Cards
                .AsNoTracking()
                .SingleOrDefaultAsync(c => c.Number == number);
        }

        public async Task<bool> ActivateAsync(long userId, long cardId, string cardCodeHash)
        {
            int activatedCards = await _context.Cards
                .Where(c => c.Id == cardId && c.CodeHash == cardCodeHash)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.UserId, userId)
                    .SetProperty(c => c.ActivationDate, DateTime.UtcNow)
                    .SetProperty(c => c.IsActivated, true));
            
            await _context.SaveChangesAsync();

            return activatedCards != 0;
        }
    }
}