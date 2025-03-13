using Domain.Aggregates.UserAggregate;
using Infrastructure.Data.Configurations.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            
            builder.Property(e => e.UserName).HasColumnName("user_name");
            builder.Property(e => e.NormalizedUserName).HasColumnName("normalized_user_name");
            builder.Property(e => e.Email).HasColumnName("email");
            builder.Property(e => e.NormalizedEmail).HasColumnName("normalized_email");
            builder.Property(e => e.EmailConfirmed).HasColumnName("email_confirmed");
            builder.Property(e => e.PasswordHash).HasColumnName("password_hash");
            builder.Property(e => e.SecurityStamp).HasColumnName("security_stamp");
            builder.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            builder.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            builder.Property(e => e.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
            builder.Property(e => e.TwoFactorEnabled).HasColumnName("two_factor_enabled");
            builder.Property(e => e.LockoutEnd).HasColumnName("lockout_end");
            builder.Property(e => e.LockoutEnabled).HasColumnName("lockout_enabled");
            builder.Property(e => e.AccessFailedCount).HasColumnName("access_failed_count");
            
            builder.Navigation(u => u.Cards).UsePropertyAccessMode(PropertyAccessMode.Field);
            
            builder
                .HasMany(u => u.Cards)
                .WithOne()
                .HasForeignKey(c => c.UserId)
                .HasConstraintName("FK_card_user_id")
                .IsRequired();

            builder
                .Property(u => u.DayOfBirth)
                .HasColumnName("day_of_birth")
                .HasConversion(new ToUtcValueConverter())
                .IsRequired(false);
            
            builder
                .Property(u => u.RegisteredAt)
                .HasColumnName("registered_at")
                .HasConversion(new ToUtcValueConverter())
                .IsRequired();
        }
    }
}