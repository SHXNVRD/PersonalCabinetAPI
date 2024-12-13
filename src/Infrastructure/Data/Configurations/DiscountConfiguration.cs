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
    public class DiscountConfiguration : IdentityConfigurationBase<Discount>
    {
        protected override void AddCustomConfiguration(EntityTypeBuilder<Discount> builder)
        {
            builder
                .HasOne(d => d.Purchase)
                .WithOne(p => p.Discount)
                .HasForeignKey<Discount>(d => d.PurchaseId)
                .IsRequired();
        }
    }
}