using Domain.Models;

namespace Application.Interfaces.Repositories;

public interface IRefundRepository
{
    Task AddAsync(Refund refund);
}