using Domain.Aggregates.PurchaseAggregate;
using Infrastructure.Data.Configurations.Base;
using Infrastructure.Data.Configurations.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PurchaseConfiguration : AggregateConfigurationBase<Purchase, Guid>
{
    protected override void AddAggregateConfiguration(EntityTypeBuilder<Purchase> builder)
    {
            builder.ToTable("purchases");
            
            builder
                .Navigation(p => p.PurchaseItems)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder
                .HasMany(p => p.PurchaseItems)
                .WithOne()
                .HasForeignKey("purchase_id")
                .HasConstraintName("FK_purchase_item_purchase_id")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder
                .HasOne<Refund>()
                .WithOne()
                .HasForeignKey<Refund>(r => r.PurchaseId)
                .HasConstraintName("FK_refund_purchase_id")
                .IsRequired(false);

            builder
                .Property(p => p.CardId)
                .HasColumnName("card_id")
                .IsRequired();

            builder
                .Property(p => p.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()")
                .HasConversion(new ToUtcValueConverter())
                .IsRequired();

            builder.Ignore(p => p.Total);
        }
}