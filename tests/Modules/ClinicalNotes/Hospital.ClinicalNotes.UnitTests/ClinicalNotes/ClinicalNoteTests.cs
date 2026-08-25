using Hospital.ClinicalNotes.Domain.ClinicalNotes;
using Xunit;

namespace Hospital.ClinicalNotes.UnitTests.ClinicalNotes;

public sealed class ClinicalNoteTests
{
    [Fact]
    public void Create_ShouldCreateClinicalNote()
    {
        var patientId = Guid.NewGuid();
        var createdAtUtc = DateTimeOffset.UtcNow;

        var note = ClinicalNote.Create(
            patientId,
            "Dr. João Silva",
            ClinicalNoteType.Medical,
            "Paciente em bom estado geral.",
            createdAtUtc);

        Assert.NotEqual(Guid.Empty, note.Id.Value);
        Assert.Equal(patientId, note.PatientId);
        Assert.Equal("Dr. João Silva", note.Professional);
        Assert.Equal(ClinicalNoteType.Medical, note.NoteType);
        Assert.Equal(
            "Paciente em bom estado geral.",
            note.Content);
        Assert.Equal(createdAtUtc, note.CreatedAtUtc);
    }

    [Fact]
    public void Create_ShouldTrimProfessionalAndContent()
    {
        var note = ClinicalNote.Create(
            Guid.NewGuid(),
            "  Dra. Maria Souza  ",
            ClinicalNoteType.Nursing,
            "  Paciente consciente e orientado.  ",
            DateTimeOffset.UtcNow);

        Assert.Equal(
            "Dra. Maria Souza",
            note.Professional);

        Assert.Equal(
            "Paciente consciente e orientado.",
            note.Content);
    }

    [Fact]
    public void Create_WithEmptyPatientId_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            ClinicalNote.Create(
                Guid.Empty,
                "Dr. João Silva",
                ClinicalNoteType.Medical,
                "Evolução clínica.",
                DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithInvalidProfessional_ShouldThrow(
        string professional)
    {
        Assert.Throws<ArgumentException>(() =>
            ClinicalNote.Create(
                Guid.NewGuid(),
                professional,
                ClinicalNoteType.Medical,
                "Evolução clínica.",
                DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Create_WithInvalidContent_ShouldThrow(
        string content)
    {
        Assert.Throws<ArgumentException>(() =>
            ClinicalNote.Create(
                Guid.NewGuid(),
                "Dr. João Silva",
                ClinicalNoteType.Medical,
                content,
                DateTimeOffset.UtcNow));
    }

    [Fact]
    public void Create_WithInvalidNoteType_ShouldThrow()
    {
        var invalidType = (ClinicalNoteType)999;

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ClinicalNote.Create(
                Guid.NewGuid(),
                "Dr. João Silva",
                invalidType,
                "Evolução clínica.",
                DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData(ClinicalNoteType.Evolution)]
    [InlineData(ClinicalNoteType.Nursing)]
    [InlineData(ClinicalNoteType.Medical)]
    [InlineData(ClinicalNoteType.Physiotherapy)]
    [InlineData(ClinicalNoteType.Nutrition)]
    [InlineData(ClinicalNoteType.Psychology)]
    [InlineData(ClinicalNoteType.Other)]
    public void Create_WithValidNoteType_ShouldCreate(
        ClinicalNoteType noteType)
    {
        var note = ClinicalNote.Create(
            Guid.NewGuid(),
            "Profissional",
            noteType,
            "Registro clínico.",
            DateTimeOffset.UtcNow);

        Assert.Equal(noteType, note.NoteType);
    }
}