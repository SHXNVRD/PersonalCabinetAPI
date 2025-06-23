using Domain.Aggregates.PurchaseAggregate;

namespace Application.Interfaces.Repositories;

public interface IPurchaseRepository
{
    Task AddAsync(Purchase purchase);
}