using Microsoft.EntityFrameworkCore;
using OpenVision.EntityFramework.DbContexts.Converters;
using OpenVision.EntityFramework.Entities;

namespace OpenVision.EntityFramework.DbContexts;

/// <summary>
/// Represents the DbContext for interacting with the application's database.
/// </summary>
public class ApplicationDbContext : DbContext
{
    #region Fields/Consts

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

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    #region Methods Overrides

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Database>()
            .HasMany(i => i.ApiKeys)
            .WithOne(t => t.Database)
            .HasForeignKey(i => i.DatabaseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Database>()
            .HasMany(i => i.ImageTargets)
            .WithOne(t => t.Database)
            .HasForeignKey(i => i.DatabaseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ApiKey>()
            .HasOne(i => i.Database)
            .WithMany(t => t.ApiKeys)
            .HasForeignKey(i => i.DatabaseId);

        modelBuilder.Entity<ImageTarget>()
            .HasOne(i => i.Database)
            .WithMany(t => t.ImageTargets)
            .HasForeignKey(i => i.DatabaseId);

        modelBuilder.Entity<Database>()
            .Property(e => e.Created)
            .HasConversion(new DateTimeOffsetConverter());

        modelBuilder.Entity<Database>()
            .Property(e => e.Updated)
            .HasConversion(new DateTimeOffsetConverter());

        modelBuilder.Entity<ApiKey>()
            .Property(e => e.Created)
            .HasConversion(new DateTimeOffsetConverter());

        modelBuilder.Entity<ApiKey>()
            .Property(e => e.Updated)
            .HasConversion(new DateTimeOffsetConverter());

        modelBuilder.Entity<ImageTarget>()
            .Property(e => e.Created)
            .HasConversion(new DateTimeOffsetConverter());

        modelBuilder.Entity<ImageTarget>()
            .Property(e => e.Updated)
            .HasConversion(new DateTimeOffsetConverter());
    }

    #endregion
}