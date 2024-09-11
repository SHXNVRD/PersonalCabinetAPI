using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<long>, long>
    {
        public override DbSet<User> Users { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<BonusSystem> BonusSystems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<Station> Stations { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new StationConfiguration());
            builder.ApplyConfiguration(new PurchaseConfiguration());
            builder.ApplyConfiguration(new PurchaseItemConfiguration());
            builder.ApplyConfiguration(new DiscountConfiguration());
            builder.ApplyConfiguration(new CategoryConfiguration());
            builder.ApplyConfiguration(new CardConfiguration());
            builder.ApplyConfiguration(new BonusSystemConfiguration());
            builder.ApplyConfiguration(new ProductConfiguration());
        }
    }
}