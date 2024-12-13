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
    public class PurchaseConfiguration : IdentityConfigurationBase<Purchase>
    {
        protected override void AddCustomConfiguration(EntityTypeBuilder<Purchase> builder)
        {
            builder
                .HasOne(p => p.Station)
                .WithMany(s => s.Purchases)
                .HasForeignKey(p => p.StationId)
                .IsRequired();

            builder
                .HasMany(p => p.PurchaseItems)
                .WithOne(pi => pi.Purchase)
                .HasForeignKey(pi => pi.PurchaseId)
                .IsRequired();

            builder
                .Property(p => p.CreatedAt)
                .HasDefaultValueSql("NOW()");
        }
    }
}