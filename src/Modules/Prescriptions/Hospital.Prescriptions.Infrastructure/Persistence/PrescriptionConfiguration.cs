using Hospital.Prescriptions.Domain.Prescriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Prescriptions.Infrastructure.Persistence;

public sealed class PrescriptionConfiguration
    : IEntityTypeConfiguration<Prescription>
{
    public void Configure(
        EntityTypeBuilder<Prescription> builder)
    {
        builder.ToTable("prescriptions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new PrescriptionId(value))
            .HasColumnName("id");

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.PrescribedAtUtc)
            .HasColumnName("prescribed_at_utc")
            .IsRequired();

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("ix_prescriptions_patient_id");

        builder.HasIndex(x => x.PrescribedAtUtc)
            .HasDatabaseName("ix_prescriptions_prescribed_at_utc");
    }
}