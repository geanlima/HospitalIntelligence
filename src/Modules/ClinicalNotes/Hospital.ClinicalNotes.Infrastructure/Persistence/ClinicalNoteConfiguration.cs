using Hospital.ClinicalNotes.Domain.ClinicalNotes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hospital.ClinicalNotes.Infrastructure.Persistence;

public sealed class ClinicalNoteConfiguration
    : IEntityTypeConfiguration<ClinicalNote>
{
    public void Configure(EntityTypeBuilder<ClinicalNote> builder)
    {
        builder.ToTable("clinical_notes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new ClinicalNoteId(value))
            .HasColumnName("id");

        builder.Property(x => x.PatientId)
            .HasColumnName("patient_id")
            .IsRequired();

        builder.Property(x => x.Professional)
            .HasColumnName("professional")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.NoteType)
            .HasColumnName("note_type")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Content)
            .HasColumnName("content")
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.HasIndex(x => x.PatientId)
            .HasDatabaseName("ix_clinical_notes_patient_id");

        builder.HasIndex(x => x.CreatedAtUtc)
            .HasDatabaseName("ix_clinical_notes_created_at_utc");
    }
}