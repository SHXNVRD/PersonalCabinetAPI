using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Aggregates;
using Domain.Aggregates.ProductAggregate;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> FindById(long id, TrackingType trackingType)
        => await _context.Products
            .SetTracking(trackingType)
            .Include(p => p.ProductPriceHistories)
            .SingleOrDefaultAsync(p => p.Id == id);

    public async Task<decimal> GetPriceAtDateAsync(long productId, DateTime date)
    {
        var price = await _context.ProductPriceHistories
            .Where(pph => pph.ProductId == productId && pph.ChangedAt <= date)
            .OrderByDescending(pph => pph.ChangedAt)
            .Select(pph => pph.Price)
            .FirstOrDefaultAsync();

        price = price != default
            ? price
            : _context.Products
                .Where(p => p.Id == productId)
                .Select(p => p.Price)
                .First();

        return price;
    }
}