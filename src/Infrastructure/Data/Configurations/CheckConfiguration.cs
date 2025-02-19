using Domain.Models;
using Infrastructure.Data.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CheckConfiguration : IdentityConfigurationBase<Check>
{
    protected override void AddCustomConfiguration(EntityTypeBuilder<Check> builder)
    {
        builder
            .Property(c => c.CreatedAt)
            .HasDefaultValueSql("NOW()");
    }
}