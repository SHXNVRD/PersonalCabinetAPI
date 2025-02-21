using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Data.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class CardConfiguration : IdentityConfigurationBase<Card>
    {

        protected override void AddCustomConfiguration(EntityTypeBuilder<Card> builder)
        {
            builder
                .Property(c => c.IsActivated)
                .HasDefaultValue(false);

            builder
                .HasMany(c => c.Purchases)
                .WithOne(p => p.Card)
                .HasForeignKey(p => p.CardId);

            builder
                .Property(c => c.Number)
                .HasMaxLength(12)
                .IsFixedLength();
        }
    }
}