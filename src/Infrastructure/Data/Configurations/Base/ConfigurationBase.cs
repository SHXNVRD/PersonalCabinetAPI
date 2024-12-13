using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Base;

public abstract class ConfigurationBase<T> : IEntityTypeConfiguration<T> where T : class
{
    public void Configure(EntityTypeBuilder<T> builder)
    {
        AddBaseConfiguration(builder);
    }

    protected abstract void AddBaseConfiguration(EntityTypeBuilder<T> builder);
}