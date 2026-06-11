using Microsoft.EntityFrameworkCore;
using WindTurbineMonitor.Api.Models;
using WindTurbineMonitor.Api.Models.Enums;

namespace WindTurbineMonitor.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Turbine> Turbines { get; set; } = null!;
    public DbSet<TurbineMetric> TurbineMetrics { get; set; } = null!;
    public DbSet<Alert> Alerts { get; set; } = null!;
    public DbSet<CommandLog> CommandLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Turbine configuration
        modelBuilder.Entity<Turbine>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
            entity.Property(e => e.MqttTopicPrefix).IsRequired().HasMaxLength(100);

            entity.HasMany(e => e.Metrics)
                .WithOne(m => m.Turbine)
                .HasForeignKey(m => m.TurbineId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Alerts)
                .WithOne(a => a.Turbine)
                .HasForeignKey(a => a.TurbineId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.CommandLogs)
                .WithOne(c => c.Turbine)
                .HasForeignKey(c => c.TurbineId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // TurbineMetric configuration
        modelBuilder.Entity<TurbineMetric>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Status).HasConversion<string>();

            // Indices for time-series query performance
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => new { e.TurbineId, e.Timestamp });
        });

        // Alert configuration
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Severity).HasConversion<string>();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);

            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => new { e.TurbineId, e.Timestamp });
        });

        // CommandLog configuration
        modelBuilder.Entity<CommandLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.IssuedByUsername).IsRequired().HasMaxLength(100);
            entity.Property(e => e.IssuedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.CommandType).HasConversion<string>();
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasIndex(e => e.IssuedAt);
            entity.HasIndex(e => new { e.TurbineId, e.IssuedAt });
        });
    }
}
