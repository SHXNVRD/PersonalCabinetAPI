using Domain.Aggregates.CardAggregate;
using Domain.Aggregates.UserAggregate;
using Infrastructure.Data.Configurations.Base;
using Infrastructure.Data.Configurations.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CardConfiguration : IdentityConfigurationBase<Card, Guid>
{
    protected override void AddCustomConfiguration(EntityTypeBuilder<Card> builder)
    {
        builder.ToTable("cards");
            
        builder
            .Navigation(c => c.Purchases)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder
            .HasOne<User>()
            .WithMany(u => u.Cards)
            .HasForeignKey(c => c.UserId)
            .HasConstraintName("FK_card_user_id");

        builder
            .Property(c => c.UserId)
            .HasColumnName("user_id");

        builder
            .HasMany(c => c.Purchases)
            .WithOne()
            .HasForeignKey(p => p.CardId)
            .HasConstraintName("FK_purchase_card_id")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder
            .Navigation(c => c.Refunds)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder
            .HasMany(c => c.Refunds)
            .WithOne()
            .HasForeignKey(r => r.CardId)
            .HasConstraintName("FK_refund_card_id")
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder
            .HasOne(c => c.Status)
            .WithMany()
            .HasForeignKey("status_id")
            .HasConstraintName("FK_card_status_id")
            .IsRequired();

        builder
            .Property(c => c.ActivatedAt)
            .HasColumnName("activated_at")
            .HasDefaultValueSql("NOW()")
            .HasConversion(new ToUtcValueConverter())
            .IsRequired(false);

        builder
            .Property(c => c.Balance)
            .HasColumnName("balance")
            .IsRequired();

        builder.ComplexProperty(
            c => c.Number,
            c => c.Property(n => n.Value)
                .HasColumnName("number")
                .HasMaxLength(12)
                .IsFixedLength()
                .IsRequired());

        builder.ComplexProperty(
            c => c.PinHash,
            c => c.Property(ph => ph.Value)
                .HasColumnName("pin_hash")
                .HasMaxLength(64)
                .IsFixedLength()
                .IsRequired());
    }
}