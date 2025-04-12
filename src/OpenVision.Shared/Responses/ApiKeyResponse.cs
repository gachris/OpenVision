using OpenVision.Shared.Types;

namespace OpenVision.Shared.Responses;

/// <summary>
/// Represents an API key response.
/// </summary>
public record ApiKeyResponse
{
    /// <summary>
    /// Gets the unique identifier of the API key.
    /// </summary>
    public required virtual Guid Id { get; init; }

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
}
