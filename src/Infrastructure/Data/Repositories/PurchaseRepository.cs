using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Aggregates;
using Domain.Aggregates.PurchaseAggregate;
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
}