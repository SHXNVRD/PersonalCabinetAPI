using Domain.Aggregates.PurchaseAggregate;
using Infrastructure.Data.Configurations.Base;
using Infrastructure.Data.Configurations.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RefundConfiguration : IdentityConfigurationBase<Refund, Guid>
{
    protected override void AddIdentityConfiguration(EntityTypeBuilder<Refund> builder)
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
            .Property(r => r.Total)
            .HasColumnName("total")
            .IsRequired();

        builder
            .Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .HasConversion(new ToUtcValueConverter())
            .IsRequired();
    }
}