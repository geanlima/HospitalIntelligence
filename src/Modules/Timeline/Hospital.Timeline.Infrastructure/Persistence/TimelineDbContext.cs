using Hospital.Timeline.Domain.Timeline;
using Microsoft.EntityFrameworkCore;

namespace Hospital.Timeline.Infrastructure.Persistence;

public sealed class TimelineDbContext : DbContext
{
    public TimelineDbContext(
        DbContextOptions<TimelineDbContext> options)
        : base(options)
    {
    }

    public DbSet<TimelineItem> TimelineItems =>
        Set<TimelineItem>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(
            new TimelineItemConfiguration());
    }
}