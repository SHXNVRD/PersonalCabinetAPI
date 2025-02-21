using Application.DTOs;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private IPurchaseRepository? _purchaseRepository;
        private IProductRepository? _productRepository;
        private ICardRepository? _cardRepository;
        private IUserRepository? _userRepository;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UnitOfWork> _logger;

        public IPurchaseRepository PurchaseRepository
        {
            get
            {
                if (_purchaseRepository == null)
                    return _purchaseRepository = new PurchaseRepository(_dbContext);
                return _purchaseRepository;
            }
        }

        public IProductRepository ProductRepository
        {
            get
            {
                if (_productRepository == null)
                    _productRepository = new ProductRepository(_dbContext);
                return _productRepository;
            }
        }

        public ICardRepository CardRepository
        {
            get
            {
                if (_cardRepository == null)
                    _cardRepository = new CardRepository(_dbContext);
                return _cardRepository;
            }
        }

        public IUserRepository UserRepository
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = new UserRepository(_dbContext);
                return _userRepository;
            }
        }

        public UnitOfWork(AppDbContext dbContext, ILogger<UnitOfWork> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(bool useIfExists = false)
        {
            var transaction = _dbContext.Database.CurrentTransaction;
            
            if (transaction == null)
                return await _dbContext.Database.BeginTransactionAsync();
            
            return useIfExists ? await Task.FromResult(transaction) : await _dbContext.Database.BeginTransactionAsync();
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