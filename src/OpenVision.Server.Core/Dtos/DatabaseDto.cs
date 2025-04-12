using OpenVision.Shared.Types;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents a data transfer object for a database.
/// </summary>
public record DatabaseDto
{
    /// <summary>
    /// Gets the unique identifier of the database.
    /// </summary>
    public required virtual Guid Id { get; init; }
    
    /// <summary>
    /// Gets the name of the database.
    /// </summary>
    public required virtual string Name { get; init; }

    /// <summary>
    /// Gets the type of the database (device or cloud).
    /// </summary>
    public required virtual DatabaseType Type { get; init; }

    /// <summary>
    /// Gets the date and time that the database was created.
    /// </summary>
    public required virtual DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the date and time that the database was last updated.
    /// </summary>
    public required virtual DateTimeOffset Updated { get; init; }

    /// <summary>
    /// Gets the collection of API keys associated with the database.
    /// </summary>
    public required virtual ICollection<ApiKeyDto> ApiKeys { get; init; }

    /// <summary>
    /// Gets the collection of targets associated with the database.
    /// </summary>
    public required virtual ICollection<TargetDto> Targets { get; init; }
}