using Domain.Aggregates.PurchaseAggregate;
using Infrastructure.Data.Configurations.Base;
using Infrastructure.Data.Configurations.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CheckConfiguration : IdentityConfigurationBase<Check, long>
{
    protected override void AddIdentityConfiguration(EntityTypeBuilder<Check> builder)
    {
        builder.ToTable("checks", t =>
            t.HasCheckConstraint("CK_Check_Single_Owner", 
                "(purchase_id IS NOT NULL AND refund_id IS NULL) OR " +
                "(purchase_id IS NULL AND refund_id IS NOT NULL)"));
        
        builder
            .HasOne<Purchase>()
            .WithOne(p => p.Check)
            .HasForeignKey<Check>("purchase_id")
            .HasConstraintName("FK_check_purchase_id")
            .IsRequired(false);

        builder
            .HasIndex("purchase_id")
            .IsUnique()
            .HasFilter("purchase_id IS NOT NULL");
        
        builder
            .HasOne<Refund>()
            .WithOne(r => r.Check)
            .HasForeignKey<Check>("refund_id")
            .HasConstraintName("FK_check_refund_id")
            .IsRequired(false);
        
        builder
            .HasIndex("refund_id")
            .IsUnique()
            .HasFilter("refund_id IS NOT NULL");
        
        builder
            .Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("NOW()")
            .HasConversion(new ToUtcValueConverter());
    }
}