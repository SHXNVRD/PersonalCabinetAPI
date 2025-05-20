using Domain.Aggregates.ProductAggregate;
using Infrastructure.Data.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProductConfiguration : AggregateConfigurationBase<Product, long>
{
    protected override void AddAggregateConfiguration(EntityTypeBuilder<Product> builder)
    {
            builder.ToTable("products");
            
            builder
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey("category_id")
                .HasConstraintName("FK_product_category_id")
                .IsRequired();

            builder
                .Navigation(p => p.ProductPriceHistories)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder
                .HasMany(p => p.ProductPriceHistories)
                .WithOne()
                .HasForeignKey("product_id")
                .HasConstraintName("FK_product_price_history_product_id")
                .IsRequired();

            builder
                .Property(p => p.Title)
                .HasColumnName("title")
                .IsRequired();

            builder
                .Property(p => p.Description)
                .HasColumnName("description")
                .IsRequired();

            builder.ComplexProperty(
                p => p.Quantity,
                p => p.Property(q => q.Value)
                    .HasColumnName("quantity")
                    .IsRequired());

            builder.Ignore(p => p.Price);
        }
}