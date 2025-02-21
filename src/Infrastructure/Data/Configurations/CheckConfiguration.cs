using System.Linq.Expressions;
using Domain.Models;
using Infrastructure.Data.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CheckConfiguration : IdentityConfigurationBase<Check>
{
    protected override void AddCustomConfiguration(EntityTypeBuilder<Check> builder)
    {
        Expression<Func<DateTime, DateTime>> convertToUtc = dateTime =>
            dateTime.Kind == DateTimeKind.Utc ? dateTime : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

        builder
            .Property(c => c.CreatedAt)
            .HasConversion(convertToUtc, convertToUtc)
            .IsRequired();
        
        builder
            .Property(c => c.CreatedAt)
            .HasDefaultValueSql("now()");
    }
}