using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Data.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class BonusSystemConfiguration : IdentityConfigurationBase<BonusSystem>
    {
        protected override void AddCustomConfiguration(EntityTypeBuilder<BonusSystem> builder)
        {
            builder
                .Property(bs => bs.Id)
                .UseIdentityAlwaysColumn();
        }
    }
}