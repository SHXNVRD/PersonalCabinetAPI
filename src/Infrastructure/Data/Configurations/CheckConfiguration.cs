using Domain.Aggregates.PurchaseAggregate;
using Infrastructure.Data.Configurations.Base;
using Infrastructure.Data.Configurations.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CheckConfiguration : IdentityConfigurationBase<Check, long>
{
    protected override void AddCustomConfiguration(EntityTypeBuilder<Check> builder)
    {
        builder.ToTable("checks");
        
        builder
            .Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasConversion(new ToUtcValueConverter());
    }
}