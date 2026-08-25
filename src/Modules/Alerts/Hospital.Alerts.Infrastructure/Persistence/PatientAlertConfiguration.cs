using Hospital.Alerts.Domain.Alerts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Alerts.Infrastructure.Persistence;

public sealed class PatientAlertConfiguration
    : IEntityTypeConfiguration<PatientAlert>
{
    public void Configure(
        EntityTypeBuilder<PatientAlert> builder)
    {
        builder.ToTable("patient_alerts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new PatientAlertId(value))
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.Type)
            .HasColumnName("type")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(x => x.Severity)
            .HasColumnName("severity")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.AcknowledgedAtUtc)
            .HasColumnName("acknowledged_at_utc");

        builder.Property(x => x.ResolvedAtUtc)
            .HasColumnName("resolved_at_utc");

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("ix_patient_alerts_patient_id");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("ix_patient_alerts_created_at_utc");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("ix_patient_alerts_status");

        builder.HasIndex(x => new
        {
            x.PatientId,
            x.Status
        })
        .HasDatabaseName(
            "ix_patient_alerts_patient_id_status");
    }
}