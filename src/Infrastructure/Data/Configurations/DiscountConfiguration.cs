using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.HasKey(d => d.Id);

            builder
                .Property(d => d.Id)
                .UseIdentityColumn();

            builder
                .HasOne(d => d.Purchase)
                .WithOne(p => p.Discount)
                .HasForeignKey<Discount>(d => d.PurchaseId)
                .IsRequired();
        }
    }
}