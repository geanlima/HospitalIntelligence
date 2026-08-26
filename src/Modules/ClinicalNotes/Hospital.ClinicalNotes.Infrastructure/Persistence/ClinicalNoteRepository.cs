using Hospital.ClinicalNotes.Application.ClinicalNotes.Abstractions;
using Hospital.ClinicalNotes.Domain.ClinicalNotes;
using Microsoft.EntityFrameworkCore;

namespace Hospital.ClinicalNotes.Infrastructure.Persistence;

public sealed class ClinicalNoteRepository
    : IClinicalNoteRepository
{
    private readonly ClinicalNotesDbContext _dbContext;

    public ClinicalNoteRepository(
        ClinicalNotesDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClinicalNote?> GetByIdAsync(
        ClinicalNoteId id,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ClinicalNotes
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        ClinicalNote clinicalNote,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.ClinicalNotes.AddAsync(
            clinicalNote,
            cancellationToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<ClinicalNote>> GetByPatientIdAsync(
        Guid patientId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.ClinicalNotes
            .AsNoTracking()
            .Where(x => x.PatientId == patientId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ClinicalNote>> SearchAsync(
        ClinicalNoteType? noteType,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ClinicalNotes
            .AsNoTracking()
            .AsQueryable();

        if (noteType.HasValue)
        {
            query = query.Where(
                x => x.NoteType == noteType.Value);
        }

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
}