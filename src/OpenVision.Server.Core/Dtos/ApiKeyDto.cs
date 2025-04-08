using OpenVision.Shared;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents a data transfer object for a api key.
/// </summary>
public class ApiKeyDto
{
    /// <summary>
    /// Gets or sets the unique identifier of the API key.
    /// </summary>
    public required virtual Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the API key string.
    /// </summary>
    public required virtual string Key { get; set; }

    /// <summary>
    /// Gets or sets the type of the API key (client or server).
    /// </summary>
    public required virtual ApiKeyType Type { get; set; }

    /// <summary>
    /// Gets or sets the date and time that the API key was last updated.
    /// </summary>
    public required virtual DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Gets or sets the date and time that the API key was created.
    /// </summary>
    public required virtual DateTimeOffset Created { get; set; }
}
