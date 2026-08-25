using Hospital.Exams.Domain.Exams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.Exams.Infrastructure.Persistence;

public sealed class ExamConfiguration
    : IEntityTypeConfiguration<Exam>
{
    public void Configure(
        EntityTypeBuilder<Exam> builder)
    {
        builder.ToTable("exams");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new ExamId(value))
            .HasColumnName("id");

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.RequestedAtUtc)
            .HasColumnName("requested_at_utc")
            .IsRequired();

        builder.Property(x => x.ResultedAtUtc)
            .HasColumnName("resulted_at_utc");

        builder.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Result)
            .HasColumnName("result");

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("ix_exams_patient_id");

        builder.HasIndex(x => x.RequestedAtUtc)
            .HasDatabaseName("ix_exams_requested_at_utc");
    }
}