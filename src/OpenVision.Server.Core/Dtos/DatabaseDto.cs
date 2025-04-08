using OpenVision.Shared;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents a data transfer object for a database.
/// </summary>
public class DatabaseDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the database.
    /// </summary>
    public required virtual Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated database.
    /// </summary>
    public virtual required Guid DatabaseId { get; set; }

    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    public required virtual string Name { get; set; }

    /// <summary>
    /// Gets or sets the type of the database (device or cloud).
    /// </summary>
    public required virtual DatabaseType Type { get; set; }

    /// <summary>
    /// Gets or sets the collection of API keys associated with the database.
    /// </summary>
    public required virtual IEnumerable<ApiKeyDto> ApiKeys { get; set; }

    /// <summary>
    /// Gets or sets the collection of targets associated with the database.
    /// </summary>
    public required virtual IEnumerable<TargetDto> Targets { get; set; } 

    /// <summary>
    /// Gets or sets the date and time that the database was created.
    /// </summary>
    public required virtual DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the date and time that the database was last updated.
    /// </summary>
    public required virtual DateTimeOffset Updated { get; set; }
}