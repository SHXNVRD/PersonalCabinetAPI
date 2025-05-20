using Domain.Aggregates.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Base;

public abstract class AggregateConfigurationBase<TAggregate, TKey> 
    : IdentityConfigurationBase<TAggregate, TKey> where TAggregate : Aggregate<TKey>
{
    protected override void AddIdentityConfiguration(EntityTypeBuilder<TAggregate> builder)
    {
        builder.Ignore(a => a.DomainEvents);
        
        AddAggregateConfiguration(builder);
    }
    
    protected abstract void AddAggregateConfiguration(EntityTypeBuilder<TAggregate> builder);
}