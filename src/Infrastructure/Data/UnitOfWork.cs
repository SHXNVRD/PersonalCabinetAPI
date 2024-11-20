using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private ICardRepository? _cardRepository;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UnitOfWork> _logger;

        public ICardRepository CardRepository
        {
            get
            {
                if (_cardRepository == null)
                    _cardRepository = new CardRepository(_dbContext);
                return _cardRepository;
            }
        }

        public UnitOfWork(AppDbContext dbContext, ILogger<UnitOfWork> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public Task<IDbContextTransaction> BeginTransactionAsync(bool useIfExists = false)
        {
            var transaction = _dbContext.Database.CurrentTransaction;
            
            if (transaction == null)
                return _dbContext.Database.BeginTransactionAsync();
            
            return useIfExists ? Task.FromResult(transaction) : _dbContext.Database.BeginTransactionAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Exception has occurred while saving changes into database: {Exception}", exception.Message);
                return false;
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