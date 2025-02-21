using Application.Purchases.Commands;
using Domain.Models;

namespace Application.Interfaces.Repositories;

public interface IPurchaseRepository
{
    Task AddAsync(Purchase purchase);
}