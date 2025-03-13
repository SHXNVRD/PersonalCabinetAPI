using Domain.Aggregates.PurchaseAggregate;
using Infrastructure.Data.Configurations.Base;
using Infrastructure.Data.Configurations.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RefundConfiguration : IdentityConfigurationBase<Refund, Guid>
{
    protected override void AddCustomConfiguration(EntityTypeBuilder<Refund> builder)
    {
        builder.ToTable("refunds");
        
        builder
            .Property(r => r.PurchaseId)
            .HasColumnName("purchase_id")
            .IsRequired();

        builder
            .Property(r => r.CardId)
            .HasColumnName("card_id")
            .IsRequired();

        builder
            .HasOne(r => r.Check)
            .WithOne()
            .HasForeignKey("refund_id")
            .HasConstraintName("FK_check_refund_id")
            .IsRequired(false);

        builder
            .Property(r => r.Total)
            .HasColumnName("total")
            .IsRequired();

        builder
            .Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasConversion(new ToUtcValueConverter())
            .IsRequired();
    }
}