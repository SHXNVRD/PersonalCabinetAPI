using Domain.Aggregates.CardAggregate;
using Infrastructure.Data.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class StatusConfiguration : IdentityConfigurationBase<Status, int>
{
    protected override void AddIdentityConfiguration(EntityTypeBuilder<Status> builder)
    {
        builder.ToTable("statuses");
        
        builder
            .Property(s => s.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder
            .Property(s => s.Title)
            .HasColumnName("title")
            .IsRequired();
    }
}