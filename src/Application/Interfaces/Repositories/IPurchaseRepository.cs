using Application.Purchases.Commands;
using Domain.Models;

namespace Application.Interfaces.Repositories;

public interface IPurchaseRepository
{
    Task AddAsync(Purchase purchase);
    Task<Purchase?> GetLatestByCardNumber(string cardNumber, TrackingType trackingType = TrackingType.NoTracking);
}