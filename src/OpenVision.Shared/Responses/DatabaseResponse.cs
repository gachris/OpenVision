using OpenVision.Shared.Types;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents a database response.
/// </summary>
public record DatabaseResponse
{
    /// <summary>
    /// Gets the unique identifier of the database.
    /// </summary>
    public virtual required Guid Id { get; init; }

    /// <summary>
    /// Gets the name of the database.
    /// </summary>
    public virtual required string Name { get; init; }

    /// <summary>
    /// Gets the type of the database (device or cloud).
    /// </summary>
    public virtual required DatabaseType Type { get; init; }

    /// <summary>
    /// Gets the date and time that the database was created.
    /// </summary>
    public virtual required DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the date and time that the database was last updated.
    /// </summary>
    public virtual required DateTimeOffset Updated { get; init; }

    /// <summary>
    /// Gets the collection of API keys associated with the database.
    /// </summary>
    public virtual required IEnumerable<ApiKeyResponse> ApiKeys { get; init; }

    /// <summary>
    /// Gets the collection of targets associated with the database.
    /// </summary>
    public virtual required IEnumerable<TargetResponse> Targets { get; init; }
}