using Application.Interfaces.Repositories;
using Domain.Aggregates.PurchaseAggregate;

namespace Infrastructure.Data.Repositories;

public class RefundRepository : IRefundRepository
{
    private readonly AppDbContext _context;

    public RefundRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Refund refund)
        => await _context.AddAsync(refund);
}