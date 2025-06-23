using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IPurchaseRepository PurchaseRepository { get; }
    IRefundRepository RefundRepository { get; }
    IProductRepository ProductRepository { get; }
    ICardRepository CardRepository { get; }
    IUserRepository UserRepository { get; }
    Task<IDbContextTransaction> BeginTransactionAsync(bool useIfExists = false);
    Task<bool> SaveChangesAsync();
}