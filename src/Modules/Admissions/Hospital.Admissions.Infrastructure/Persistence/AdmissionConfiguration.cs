using Hospital.Admissions.Domain.Admissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Admissions.Infrastructure.Persistence;

public sealed class AdmissionConfiguration
    : IEntityTypeConfiguration<Admission>
{
    public void Configure(
        EntityTypeBuilder<Admission> builder)
    {
        builder.ToTable("admissions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => new AdmissionId(value))
            .ValueGeneratedNever();

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.AdmissionDate)
            .HasColumnName("admission_date")
            .IsRequired();

        builder.Property(x => x.DischargeDate)
            .HasColumnName("discharge_date");

        builder.Property(x => x.Unit)
            .HasColumnName("unit")
            .HasMaxLength(100);

        builder.Property(x => x.Bed)
            .HasColumnName("bed")
            .HasMaxLength(50);

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("ix_admissions_patient_id");
    }
}