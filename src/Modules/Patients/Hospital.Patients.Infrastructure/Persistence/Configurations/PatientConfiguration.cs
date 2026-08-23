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
            .HasColumnName("id")
            .HasConversion(
                id => id.Value,
                value => new PatientId(value))
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.BirthDate)
            .HasColumnName("birth_date")
            .IsRequired();

        builder.Property(x => x.Gender)
            .HasColumnName("gender")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at_utc")
            .IsRequired();

        builder.OwnsOne(
            x => x.ExternalIdentifier,
            owned =>
            {
                owned.Property(x => x.SourceSystem)
                    .HasColumnName("source_system")
                    .HasMaxLength(50);

                owned.Property(x => x.ExternalId)
                    .HasColumnName("external_id")
                    .HasMaxLength(100);

                owned.HasIndex(
                        x => new
                        {
                            x.SourceSystem,
                            x.ExternalId
                        })
                    .IsUnique()
                    .HasDatabaseName(
                        "ux_patients_external_identifier");
            });
    }
}