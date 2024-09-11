using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class BonusSystemConfiguration : IEntityTypeConfiguration<BonusSystem>
    {
        public void Configure(EntityTypeBuilder<BonusSystem> builder)
        {
            builder.HasKey(bs => bs.Id);

            builder
                .Property(bs => bs.Id)
                .UseIdentityAlwaysColumn();
        }
    }
}