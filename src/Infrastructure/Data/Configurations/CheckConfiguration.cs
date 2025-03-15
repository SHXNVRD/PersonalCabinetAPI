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
            .HasOne<Purchase>()
            .WithOne(p => p.Check)
            .HasForeignKey<Check>("purchase_id")
            .HasConstraintName("FK_check_purchase_id")
            .IsRequired(false);
        
        builder
            .HasOne<Refund>()
            .WithOne(r => r.Check)
            .HasForeignKey<Check>("refund_id")
            .HasConstraintName("FK_check_refund_id")
            .IsRequired(false);
        
        builder
            .Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasConversion(new ToUtcValueConverter());
    }
}