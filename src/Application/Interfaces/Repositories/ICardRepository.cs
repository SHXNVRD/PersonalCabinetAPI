using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Aggregates.CardAggregate;
using Domain.Shared.ValueObjects;

namespace Application.Interfaces.Repositories;

public interface ICardRepository
{
    Task<Card?> FindByNumberAsync(CardNumber number, TrackingType trackingType = TrackingType.NoTracking);
    Task<Card?> FindByNumberWithPurchasesAndRefundsAsync(CardNumber number, TrackingType trackingType = TrackingType.NoTracking);
    Task<Card?> FindByIdAsync(Guid id, TrackingType trackingType = TrackingType.NoTracking);
    Task<Card?> FindByUserIdAsync(Guid id, TrackingType trackingType = TrackingType.NoTracking);
    void UpdateStatus(Card card);
}