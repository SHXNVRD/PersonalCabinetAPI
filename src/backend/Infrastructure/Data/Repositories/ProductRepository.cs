using Application.Interfaces;
using Application.Interfaces.Repositories;
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

    public async Task<Product?> FindById(long id, TrackingType trackingType = TrackingType.NoTracking)
        => await _context.Products
            .SetTracking(trackingType)
            .Include(p => p.ProductPriceHistories)
            .Include(p => p.Category)
            .SingleOrDefaultAsync(p => p.Id == id);

    public async Task AddAsync(Product product)
    {
        _context.Attach(product.Category);

        await _context.Products.AddAsync(product);
    }

    public async Task<bool> RemoveAsync(long id)
    {
        var deletedRowsCount = await _context.Products
            .Where(p => p.Id == id)
            .ExecuteDeleteAsync();

        return deletedRowsCount > 0;
    }

    public void Update(Product product)
    {
        _context.Attach(product.Category);
    }
}