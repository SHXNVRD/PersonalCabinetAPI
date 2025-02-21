using System.Linq.Expressions;
using Domain.Models;
using Domain.Models.Base;
using Infrastructure.Data.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RefundConfiguration : IdentityConfigurationBase<Refund>
{
    protected override void AddCustomConfiguration(EntityTypeBuilder<Refund> builder)
    {
        Expression<Func<DateTime, DateTime>> convertToUtc = dateTime =>
            dateTime.Kind == DateTimeKind.Utc ? dateTime : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        
        builder
            .HasMany(r => r.RefundItems)
            .WithOne(ri => ri.Refund)
            .HasForeignKey(ri => ri.RefundId)
            .IsRequired();

        builder
            .HasOne(r => r.Check)
            .WithOne(c => c.Refund)
            .HasForeignKey<Check>(c => c.RefundId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder
            .Property(r => r.CreatedAt)
            .HasConversion(convertToUtc, convertToUtc)
            .IsRequired();

        builder
            .Property(r => r.CreatedAt)
            .HasDefaultValueSql("now()");
    }
}