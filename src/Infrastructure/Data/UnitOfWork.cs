using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Data
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private ICardRepository? _cardRepository;
        private readonly AppDbContext _dbContext;
        public SaveChangesResult LastSaveChangesResult { get; }

        public ICardRepository CardRepository
        {
            get
            {
                if (_cardRepository == null)
                    _cardRepository = new CardRepository(_dbContext);
                return _cardRepository;
            }
        }

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            LastSaveChangesResult = new SaveChangesResult();
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(bool useIfExists = false)
        {
            var transaction = _dbContext.Database.CurrentTransaction;
            
            if (transaction == null)
                return _dbContext.Database.BeginTransactionAsync();
            
            return useIfExists ? Task.FromResult(transaction) : _dbContext.Database.BeginTransactionAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await _dbContext.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                LastSaveChangesResult.Exception = exception;
                return 0;
            }
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}