using Application.DTOs;
using Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICardRepository CardRepository { get; }
        IUserRepository UserRepository { get; }
        Task<IDbContextTransaction> BeginTransactionAsync(bool useIfExists = false);
        Task<bool> SaveChangesAsync();
    }
}