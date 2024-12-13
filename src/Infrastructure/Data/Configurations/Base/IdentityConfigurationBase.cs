using Domain.Models.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Data.Configurations.Base;

public abstract class IdentityConfigurationBase<T> : ConfigurationBase<T> where T : Identity
{
    protected override void AddBaseConfiguration(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .IsRequired();
        
        AddCustomConfiguration(builder);
    }

    protected abstract void AddCustomConfiguration(EntityTypeBuilder<T> builder);
}