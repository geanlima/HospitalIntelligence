using Hospital.Patients.Domain.Patients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Patients.Infrastructure.Persistence.Configurations;

public sealed class PatientConfiguration
    : IEntityTypeConfiguration<Patient>
{
    public void Configure(
        EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patients");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new PatientId(value));

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.BirthDate)
            .IsRequired();

        builder.Property(x => x.Gender)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ExternalId)
            .HasMaxLength(100);

        builder.Property(x => x.SourceSystem)
            .HasMaxLength(50);

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(
                x => new
                {
                    x.SourceSystem,
                    x.ExternalId
                })
            .IsUnique()
            .HasFilter(
                "\"SourceSystem\" IS NOT NULL AND \"ExternalId\" IS NOT NULL");
    }
}