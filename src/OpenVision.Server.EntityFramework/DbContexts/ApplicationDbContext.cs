using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OpenVision.Server.EntityFramework.Entities;

namespace OpenVision.Server.EntityFramework.DbContexts;

/// <summary>
/// Represents the DbContext for interacting with the application's database.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the DbSet of API keys in the database.
    /// </summary>
    public DbSet<ApiKey> ApiKeys { get; set; }

    /// <summary>
    /// Gets or sets the DbSet of databases in the database.
    /// </summary>
    public DbSet<Database> Databases { get; set; }

    /// <summary>
    /// Gets or sets the DbSet of image targets in the database.
    /// </summary>
    public DbSet<ImageTarget> ImageTargets { get; set; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure entity mappings and relationships here (if needed)
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApiKey>()
                    .Property(e => e.Created)
                    .HasConversion(new DateTimeOffsetConverter());

        modelBuilder.Entity<ApiKey>()
                    .Property(e => e.Updated)
                    .HasConversion(new DateTimeOffsetConverter());

        modelBuilder.Entity<Database>()
                    .Property(e => e.Created)
                    .HasConversion(new DateTimeOffsetConverter());

        modelBuilder.Entity<Database>()
                    .Property(e => e.Updated)
                    .HasConversion(new DateTimeOffsetConverter());

        modelBuilder.Entity<ImageTarget>()
                    .Property(e => e.Created)
                    .HasConversion(new DateTimeOffsetConverter());

        modelBuilder.Entity<ImageTarget>()
                    .Property(e => e.Updated)
                    .HasConversion(new DateTimeOffsetConverter());
    }
}

/// <summary>
/// Converts between <see cref="DateTimeOffset"/> and <see cref="DateTime"/>.
/// </summary>
public class DateTimeOffsetConverter : ValueConverter<DateTimeOffset, DateTime>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateTimeOffsetConverter"/> class.
    /// </summary>
    public DateTimeOffsetConverter() : base(
        // Converts DateTimeOffset to DateTime (UTC DateTime).
        v => v.UtcDateTime,
        // Converts DateTime to DateTimeOffset with zero offset.
        v => new DateTimeOffset(v, TimeSpan.Zero))
    {
    }
}