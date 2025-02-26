using Domain.Models;
using Infrastructure.Data.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProductPriceHistoryConfiguration : IdentityConfigurationBase<ProductPriceHistory>
{
    protected override void AddCustomConfiguration(EntityTypeBuilder<ProductPriceHistory> builder)
    {
        builder
            .HasOne(pph => pph.Product)
            .WithMany(p => p.ProductPriceHistories)
            .HasForeignKey(pph => pph.ProductId);

        builder
            .Property(pph => pph.ChangedAt)
            .HasDefaultValueSql("now()")
            .IsRequired();

        builder
            .HasIndex(pph => new {pph.ProductId, pph.ChangedAt})
            .IsDescending(false, true);
    }
}