using System.Reflection;
using Domain.Aggregates.CardAggregate;
using Domain.Aggregates.ProductAggregate;
using Domain.Aggregates.PurchaseAggregate;
using Domain.Aggregates.UserAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public override DbSet<User> Users { get; set; } = null!;
        public DbSet<Card> Cards { get; set; } = null!;
        public DbSet<Check> Checks { get; set; } = null!;
        public DbSet<Refund> Refunds { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductPriceHistory> ProductPriceHistories { get; set; } = null!;
        public DbSet<Purchase> Purchases { get; set; } = null!;
        public DbSet<PurchaseItem> PurchaseItems { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Entity<IdentityRole<Guid>>(r =>
            {
                r.ToTable("roles");
                
                r.Property(e => e.Id).HasColumnName("id");
                r.Property(e => e.Name).HasColumnName("name");
                r.Property(e => e.NormalizedName).HasColumnName("normalized_name");
                r.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            });
            
            builder.Entity<IdentityUserRole<Guid>>(ur =>
            {
                ur.ToTable("user_roles");
                
                ur.Property(e => e.UserId).HasColumnName("user_id");
                ur.Property(e => e.RoleId).HasColumnName("role_id");
                
                ur.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK_user_roles");
            });
            
            builder.Entity<IdentityUserClaim<Guid>>(uc =>
            {
                uc.ToTable("user_claims");
        
                uc.Property(e => e.Id).HasColumnName("id");
                uc.Property(e => e.UserId).HasColumnName("user_id");
                uc.Property(e => e.ClaimType).HasColumnName("claim_type");
                uc.Property(e => e.ClaimValue).HasColumnName("claim_value");
            });
            
            builder.Entity<IdentityRoleClaim<Guid>>(rc =>
            {
                rc.ToTable("role_claims");
        
                rc.Property(e => e.Id).HasColumnName("id");
                rc.Property(e => e.RoleId).HasColumnName("role_id");
                rc.Property(e => e.ClaimType).HasColumnName("claim_type");
                rc.Property(e => e.ClaimValue).HasColumnName("claim_value");
            });

            builder.Entity<IdentityUserLogin<Guid>>(ul =>
            {
                ul.ToTable("user_logins");
        
                ul.Property(e => e.LoginProvider).HasColumnName("login_provider");
                ul.Property(e => e.ProviderKey).HasColumnName("provider_key");
                ul.Property(e => e.ProviderDisplayName).HasColumnName("provider_display_name");
                ul.Property(e => e.UserId).HasColumnName("user_id");
        
                ul.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("PK_user_logins");
            });

            builder.Entity<IdentityUserToken<Guid>>(ut =>
            {
                ut.ToTable("user_tokens");
        
                ut.Property(e => e.UserId).HasColumnName("user_id");
                ut.Property(e => e.LoginProvider).HasColumnName("login_provider");
                ut.Property(e => e.Name).HasColumnName("name");
                ut.Property(e => e.Value).HasColumnName("value");
        
                ut.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("PK_user_tokens");
            });
            
            builder.Entity<IdentityUserRole<Guid>>()
                .HasOne<IdentityRole<Guid>>()
                .WithMany()
                .HasForeignKey(e => e.RoleId)
                .HasConstraintName("FK_user_roles_role_id");

            builder.Entity<IdentityUserRole<Guid>>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_user_roles_user_id");
            
            builder.Entity<User>()
                .HasIndex(u => u.NormalizedUserName)
                .HasDatabaseName("IX_users_normalized_user_name")
                .IsUnique();

            builder.Entity<User>()
                .HasIndex(u => u.NormalizedEmail)
                .HasDatabaseName("IX_users_normalized_email");

            builder.Entity<IdentityRole<Guid>>()
                .HasIndex(r => r.NormalizedName)
                .HasDatabaseName("IX_roles_normalized_name")
                .IsUnique();
        }
    }
}