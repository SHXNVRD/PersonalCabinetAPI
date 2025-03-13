using Domain.Aggregates.PurchaseAggregate;

namespace Application.Interfaces.Repositories;

public interface IRefundRepository
{
    Task AddAsync(Refund refund);
}