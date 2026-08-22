using Hospital.Patients.Domain.Patients;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Patients.Infrastructure.Persistence;

public sealed class PatientsDbContext : DbContext
{
    public PatientsDbContext(
        DbContextOptions<PatientsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients => Set<Patient>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(PatientsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}