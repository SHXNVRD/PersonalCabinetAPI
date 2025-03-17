using Domain.Aggregates.ProductAggregate;
using Infrastructure.Data.Configurations.Base;
using Infrastructure.Data.Configurations.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProductPriceHistoryConfiguration : IdentityConfigurationBase<ProductPriceHistory, Guid>
{
    protected override void AddCustomConfiguration(EntityTypeBuilder<ProductPriceHistory> builder)
    {
        builder.ToTable("product_price_histories");
        
        
        builder
            .Property(pph => pph.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .HasConversion(new ToUtcValueConverter())
            .IsRequired();

        builder
            .Property(pph => pph.Price)
            .HasColumnName("price")
            .IsRequired();
    }
}