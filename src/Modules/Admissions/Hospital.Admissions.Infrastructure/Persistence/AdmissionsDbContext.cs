using Hospital.Admissions.Domain.Admissions;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Admissions.Infrastructure.Persistence;

public sealed class AdmissionsDbContext
    : DbContext
{
    public AdmissionsDbContext(
        DbContextOptions<AdmissionsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Admission> Admissions
        => Set<Admission>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AdmissionsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}