using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<long>, long>
    {
        public DbSet<User> Users { get; set; } = null!;
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}