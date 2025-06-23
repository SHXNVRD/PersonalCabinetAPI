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
            .Property(r => r.PurchaseItemId)
            .HasColumnName("purchase_item_id")
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
            .HasOne<PurchaseItem>()
            .WithMany()
            .HasForeignKey(r => r.PurchaseItemId)
            .HasConstraintName("FK_refund_purchase_item_id")
            .IsRequired();

        builder
            .ComplexProperty(
                r => r.Quantity,
                r => r.Property(q => q.Value)
                    .HasColumnName("quantity")
                    .IsRequired());

        builder
            .Property(r => r.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .HasConversion(new ToUtcValueConverter())
            .IsRequired();
    }
}