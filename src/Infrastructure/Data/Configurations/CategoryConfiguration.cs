using Domain.Aggregates.ProductAggregate;
using Infrastructure.Data.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IdentityConfigurationBase<Category, Guid>
    {
        protected override void AddCustomConfiguration(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories");
            
            builder
                .Property(c => c.Title)
                .HasColumnName("title")
                .IsRequired();
        }
    }
}