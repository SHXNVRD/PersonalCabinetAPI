using Infrastructure.Data.Configurations.Converters;
using Infrastructure.Data.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
{
    public void Configure(EntityTypeBuilder<OutboxEvent> builder)
    {
        builder.ToTable("outboxes");

        builder.HasKey(oe => oe.EventId);

        builder
            .Property(oe => oe.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder
            .Property(oe => oe.Content)
            .HasColumnName("content")
            .IsRequired();

        builder
            .Property(oe => oe.Type)
            .HasColumnName("type")
            .IsRequired();

        builder
            .Property(oe => oe.CreatedAt)
            .HasColumnName("created_at")
            .HasConversion(new ToUtcValueConverter())
            .IsRequired();

        builder
            .Property(oe => oe.CompletedAt)
            .HasColumnName("completed_at")
            .HasConversion(new ToUtcValueConverter())
            .IsRequired(false);
    }
}