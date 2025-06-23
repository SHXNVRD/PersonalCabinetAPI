using Domain.Aggregates.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Base;

public abstract class IdentityConfigurationBase<TEntity, TKey> : ConfigurationBase<TEntity> where TEntity : Identity<TKey>
{
    protected override void AddBaseConfiguration(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder
            .Property(x => x.Id)
            .HasColumnName("id")
            .IsRequired();
        
        AddIdentityConfiguration(builder);
    }

    protected abstract void AddIdentityConfiguration(EntityTypeBuilder<TEntity> builder);
}