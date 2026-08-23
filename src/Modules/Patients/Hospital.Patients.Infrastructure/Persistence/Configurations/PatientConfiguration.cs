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
                value => new PatientId(value))
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.BirthDate)
            .IsRequired();

        builder.Property(x => x.Gender)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
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
                    .IsUnique();
            });
    }
}