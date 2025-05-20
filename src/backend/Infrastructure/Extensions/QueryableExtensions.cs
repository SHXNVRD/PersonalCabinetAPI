using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> SetTracking<TEntity>(
        this IQueryable<TEntity> source,
        TrackingType trackingType = TrackingType.Tracking)
        where TEntity : class
    {
        return trackingType switch
        {
            TrackingType.NoTracking => source.AsNoTracking(),
            TrackingType.NoTrackingWithIdentityResolution => source.AsNoTrackingWithIdentityResolution(),
            _ => source
        };
    }
}