using Hospital.VitalSigns.Domain.VitalSigns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.VitalSigns.Infrastructure.Persistence;

public sealed class VitalSignConfiguration
    : IEntityTypeConfiguration<VitalSign>
{
    public void Configure(
        EntityTypeBuilder<VitalSign> builder)
    {
        builder.ToTable("vital_signs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new VitalSignId(value))
            .HasColumnName("id");

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.MeasuredAtUtc)
            .HasColumnName("measured_at_utc")
            .IsRequired();

        builder.Property(x => x.Temperature)
            .HasColumnName("temperature")
            .HasPrecision(5, 2);

        builder.Property(x => x.HeartRate)
            .HasColumnName("heart_rate");

        builder.Property(x => x.RespiratoryRate)
            .HasColumnName("respiratory_rate");

        builder.Property(x => x.SystolicBloodPressure)
            .HasColumnName("systolic_blood_pressure");

        builder.Property(x => x.DiastolicBloodPressure)
            .HasColumnName("diastolic_blood_pressure");

        builder.Property(x => x.OxygenSaturation)
            .HasColumnName("oxygen_saturation")
            .HasPrecision(5, 2);

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("ix_vital_signs_patient_id");

        builder.HasIndex(x => x.MeasuredAtUtc)
            .HasDatabaseName("ix_vital_signs_measured_at_utc");
    }
}