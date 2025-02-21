using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.Repositories
{
    public interface ICardRepository
    {
        Task<Card?> FindByNumberAsync(string number, TrackingType trackingType = TrackingType.NoTracking);
        Task<Card?> FindByIdAsync(long id, TrackingType trackingType = TrackingType.NoTracking);
        Task<Card?> FindByUserIdAsync(long id, TrackingType trackingType = TrackingType.NoTracking);
        Task<bool> ActivateAsync(long userId, string number, string pinCodeHash);
        Task<bool> DeactivateAsync(string number);
    }
}