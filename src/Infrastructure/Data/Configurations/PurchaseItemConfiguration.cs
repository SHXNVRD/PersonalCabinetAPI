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
    public class PurchaseItemConfiguration : IdentityConfigurationBase<PurchaseItem>
    {
        protected override void AddCustomConfiguration(EntityTypeBuilder<PurchaseItem> builder)
        {
            builder
                .HasOne(pi => pi.Product)
                .WithMany(p => p.PurchaseItems)
                .HasForeignKey(pi => pi.ProductId)
                .IsRequired();
        }
    }
}