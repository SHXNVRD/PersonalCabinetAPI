using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.HasKey(p => p.Id);

            builder 
                .Property(p => p.Id)
                .UseIdentityColumn();
            
            builder
                .HasOne(p => p.Station)
                .WithMany(s => s.Purchases)
                .HasForeignKey(p => p.StationId);

            builder
                .HasMany(p => p.PurchaseItems)
                .WithOne(pi => pi.Purchase)
                .HasForeignKey(pi => pi.PurchaseId)
                .IsRequired();

            builder
                .Property(p => p.Date)
                .HasDefaultValueSql("NOW()");
        }
    }
}