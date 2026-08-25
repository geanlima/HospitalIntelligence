using Hospital.ClinicalNotes.Domain.ClinicalNotes;
using Microsoft.EntityFrameworkCore;

namespace Hospital.ClinicalNotes.Infrastructure.Persistence;

public sealed class ClinicalNotesDbContext : DbContext
{
    public ClinicalNotesDbContext(
        DbContextOptions<ClinicalNotesDbContext> options)
        : base(options)
    {
    }

    public DbSet<ClinicalNote> ClinicalNotes => Set<ClinicalNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ClinicalNotesDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}