using Hospital.Alerts.Domain.Alerts;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Alerts.Infrastructure.Persistence;

public sealed class AlertsDbContext : DbContext
{
    public AlertsDbContext(
        DbContextOptions<AlertsDbContext> options)
        : base(options)
    {
    }

    public DbSet<PatientAlert> Alerts =>
        Set<PatientAlert>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(
            new PatientAlertConfiguration());
    }
}