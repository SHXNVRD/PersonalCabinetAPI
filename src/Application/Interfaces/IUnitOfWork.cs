using Application.DTOs;
using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        SaveChangesResult LastSaveChangesResult { get; }
        ICardRepository CardRepository { get; }
        Task<IDbContextTransaction> BeginTransactionAsync(bool useIfExists = false);
        Task<int> SaveChangesAsync();
    }
}