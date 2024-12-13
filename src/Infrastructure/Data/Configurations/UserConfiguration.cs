using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            Expression<Func<DateTime, DateTime>> convertToUtc = dateTime =>
                dateTime.Kind == DateTimeKind.Utc ? dateTime : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);

            builder
                .Property(u => u.RegisteredAt)
                .HasConversion(convertToUtc, convertToUtc)
                .IsRequired();
            
            builder
                .Property(u => u.RegisteredAt)
                .HasDefaultValueSql("now() at time zone 'utc'");

            builder
                .HasMany(u => u.Cards)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId);
        }
    }
}