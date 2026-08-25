using Hospital.VitalSigns.Domain.VitalSigns;
using Microsoft.EntityFrameworkCore;

namespace Hospital.VitalSigns.Infrastructure.Persistence;

public sealed class VitalSignsDbContext : DbContext
{
    public VitalSignsDbContext(
        DbContextOptions<VitalSignsDbContext> options)
        : base(options)
    {
    }

    public DbSet<VitalSign> VitalSigns => Set<VitalSign>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(VitalSignsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}