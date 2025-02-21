using Application.Interfaces.Repositories;
using Domain.Models;

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
}