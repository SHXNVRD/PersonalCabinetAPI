using Domain.Aggregates.PurchaseAggregate;
using Infrastructure.Data.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class PurchaseItemConfiguration : IdentityConfigurationBase<PurchaseItem, Guid>
{
    protected override void AddIdentityConfiguration(EntityTypeBuilder<PurchaseItem> builder)
    {
            builder.ToTable("purchase_items");

            builder
                .HasOne(pi => pi.Product)
                .WithMany()
                .HasForeignKey("product_id")
                .HasConstraintName("FK_purchase_item_product_id")
                .IsRequired();
            
            builder
                .Property(pi => pi.ProductPriceAtPurchase)
                .HasColumnName("product_price_at_purchase")
                .IsRequired();

            builder.ComplexProperty(
                pi => pi.Quantity,
                pi => pi.Property(q => q.Value)
                    .HasColumnName("quantity")
                    .IsRequired());

            builder.Ignore(pi => pi.Total);
        }
}