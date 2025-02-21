using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Models;
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
            .SingleOrDefaultAsync(p => p.Id == id);
}