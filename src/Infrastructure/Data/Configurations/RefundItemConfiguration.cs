using Domain.Models;
using Infrastructure.Data.Configurations.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RefundItemConfiguration : IdentityConfigurationBase<RefundItem>
{
    protected override void AddCustomConfiguration(EntityTypeBuilder<RefundItem> builder)
    { }
}