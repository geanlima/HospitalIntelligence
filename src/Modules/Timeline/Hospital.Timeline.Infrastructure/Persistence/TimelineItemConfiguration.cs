using Hospital.Timeline.Domain.Timeline;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Timeline.Infrastructure.Persistence;

public sealed class TimelineItemConfiguration
    : IEntityTypeConfiguration<TimelineItem>
{
    public void Configure(
        EntityTypeBuilder<TimelineItem> builder)
    {
        builder.ToTable("timeline_items");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new TimelineItemId(value))
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.OccurredAtUtc)
            .HasColumnName("occurred_at_utc")
            .IsRequired();

        builder.Property(x => x.Type)
            .HasColumnName("type")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(2000)
            .IsRequired();

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName(
                "ix_timeline_items_patient_id");

        builder.HasIndex(x => x.OccurredAtUtc)
            .HasDatabaseName(
                "ix_timeline_items_occurred_at_utc");

        builder.HasIndex(x => new
        {
            x.PatientId,
            x.OccurredAtUtc
        })
        .HasDatabaseName(
            "ix_timeline_items_patient_id_occurred_at_utc");
    }
}