using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.Repositories
{
    public interface ICardRepository
    {
        Task<Card?> FindByNumberAsync(int number);
        Task<Card?> FindByIdAsync(long id);
        Task<Card?> FindByUserIdAsync(long id);
        Task<bool> ActivateAsync(long userId, long cardId, string cardCodeHash);
        Task<bool> DeactivateAsync(int number);
    }
}