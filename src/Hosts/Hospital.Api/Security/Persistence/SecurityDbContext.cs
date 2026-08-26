using Microsoft.EntityFrameworkCore;

namespace Hospital.Api.Security.Persistence;

public sealed class SecurityAuditEntryEntity
{
    public Guid Id { get; set; }

    public DateTimeOffset OccurredAtUtc { get; set; }

    public string? UserName { get; set; }

    public string Action { get; set; } = string.Empty;

    public string Resource { get; set; } = string.Empty;

    public string? CorrelationId { get; set; }

    public string Details { get; set; } = string.Empty;
}

public sealed class SecurityDbContext : DbContext
{
    public SecurityDbContext(DbContextOptions<SecurityDbContext> options)
        : base(options)
    {
    }

    public DbSet<SecurityAuditEntryEntity> AuditEntries =>
        Set<SecurityAuditEntryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SecurityAuditEntryEntity>(entity =>
        {
            entity.ToTable("security_audit_entries");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.UserName).HasMaxLength(200);
            entity.Property(x => x.Action).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Resource).HasMaxLength(500).IsRequired();
            entity.Property(x => x.CorrelationId).HasMaxLength(100);
            entity.Property(x => x.Details).HasMaxLength(2000).IsRequired();
            entity.HasIndex(x => x.OccurredAtUtc);
        });
    }
}
