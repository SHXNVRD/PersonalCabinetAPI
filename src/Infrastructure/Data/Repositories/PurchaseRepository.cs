using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Models;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly AppDbContext _context;

    public PurchaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Purchase purchase)
        => await _context.Purchases.AddAsync(purchase);

    public async Task<Purchase?> GetLatestByCardNumber(string cardNumber, TrackingType trackingType = TrackingType.NoTracking)
        => await _context.Purchases
            .SetTracking(trackingType)
            .OrderByDescending(p => p.CreatedAt)
            .Include(p => p.PurchaseItems)
            .ThenInclude(pi => pi.Product)
            .ThenInclude(prod => prod!.ProductPriceHistories).AsSplitQuery()
            .Include(p => p.Card)
            .FirstOrDefaultAsync(p => p.Card != null && p.Card.Number == cardNumber);
}