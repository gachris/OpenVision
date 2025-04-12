using OpenVision.Shared.Types;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents a data transfer object for a API key.
/// </summary>
public record ApiKeyDto
{
    /// <summary>
    /// Gets the unique identifier of the API key.
    /// </summary>
    public required virtual Guid Id { get; init; }

    /// <summary>
    /// Gets the unique identifier of the database that the API key belongs to.
    /// </summary>
    public required virtual Guid DatabaseId { get; init; }

    /// <summary>
    /// Gets the API key string.
    /// </summary>
    public required virtual string Key { get; init; }

    /// <summary>
    /// Gets the type of the API key (client or server).
    /// </summary>
    public required virtual ApiKeyType Type { get; init; }

    /// <summary>
    /// Gets the date and time that the API key was last updated.
    /// </summary>
    public required virtual DateTimeOffset Updated { get; init; }

    /// <summary>
    /// Gets the date and time that the API key was created.
    /// </summary>
    public required virtual DateTimeOffset Created { get; init; }

    /// <summary>
    /// Gets the database to which the API key belongs.
    /// </summary>
    public virtual DatabaseDto? Database { get; init; }
}
