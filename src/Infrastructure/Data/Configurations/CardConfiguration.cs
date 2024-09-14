using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.HasKey(c => c.Id);

            builder
                .Property(c => c.Id)
                .UseIdentityColumn();

            builder
                .Property(c => c.IsActivated)
                .HasDefaultValue(false);

            builder
                .HasOne(c => c.BonusSystem)
                .WithMany(bs => bs.Cards)
                .HasForeignKey(c => c.BonusSystemId);

            builder
                .HasMany(c => c.Discounts)
                .WithOne(d => d.Card)
                .HasForeignKey(d => d.CardId)
                .IsRequired();

            builder
                .HasMany(c => c.Purchases)
                .WithOne(p => p.Card)
                .HasForeignKey(p => p.CardId);
        }
    }
}