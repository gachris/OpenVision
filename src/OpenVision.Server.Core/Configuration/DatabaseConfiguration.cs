namespace OpenVision.Server.Core.Configuration;

/// <summary>
/// Represents the configuration settings for the database provider.
/// </summary>
public class DatabaseConfiguration
{
    /// <summary>
    /// Gets or sets the type of database provider.
    /// </summary>
    public virtual DatabaseProviderType ProviderType { get; set; }

    /// <summary>
    /// Gets or sets the connection name of the database.
    /// </summary>
    public virtual string ConnectionName { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether to use a pooled DbContext.
    /// </summary>
    public virtual bool UsePooledDbContext { get; set; }
}