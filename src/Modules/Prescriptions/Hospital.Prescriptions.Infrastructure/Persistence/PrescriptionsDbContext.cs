using Hospital.Prescriptions.Domain.Prescriptions;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Prescriptions.Infrastructure.Persistence;

public sealed class PrescriptionsDbContext : DbContext
{
    public PrescriptionsDbContext(
        DbContextOptions<PrescriptionsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Prescription> Prescriptions => Set<Prescription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(PrescriptionsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}