using Hospital.Exams.Domain.Exams;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Exams.Infrastructure.Persistence;

public sealed class ExamsDbContext : DbContext
{
    public ExamsDbContext(
        DbContextOptions<ExamsDbContext> options)
        : base(options)
    {
    }

    public DbSet<Exam> Exams => Set<Exam>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ExamsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}