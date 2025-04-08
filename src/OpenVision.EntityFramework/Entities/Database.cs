using OpenVision.Shared;

namespace OpenVision.EntityFramework.Entities;

/// <summary>
/// Represents a database entity.
/// </summary>
public partial class Database
{
    /// <summary>
    /// Gets or sets the unique identifier of the database.
    /// </summary>
    public virtual required Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user ID associated with the database.
    /// </summary>
    public virtual required string UserId { get; set; }

    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    public virtual required string Name { get; set; }

    /// <summary>
    /// Gets or sets the type of the database.
    /// </summary>
    public virtual required DatabaseType Type { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the database was created.
    /// </summary>
    public virtual required DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the database was last updated.
    /// </summary>
    public virtual required DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Gets or sets the collection of API keys associated with this database.
    /// </summary>
    public virtual IList<ApiKey> ApiKeys { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of image targets associated with this database.
    /// </summary>
    public virtual IList<ImageTarget> ImageTargets { get; set; } = [];
}