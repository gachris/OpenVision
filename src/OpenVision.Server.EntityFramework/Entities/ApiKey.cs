using OpenVision.Shared;

namespace OpenVision.Server.EntityFramework.Entities;

/// <summary>
/// Represents an API key entity.
/// </summary>
public class ApiKey
{
    /// <summary>
    /// Gets or sets the unique identifier of the API key.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated database.
    /// </summary>
    public Guid DatabaseId { get; set; }

    /// <summary>
    /// Gets or sets the API key value.
    /// </summary>
    public required string Key { get; set; }

    /// <summary>
    /// Gets or sets the type of the API key.
    /// </summary>
    public ApiKeyType Type { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the API key was created.
    /// </summary>
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the API key was last updated.
    /// </summary>
    public DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Gets or sets the associated database entity navigation property.
    /// </summary>
    public required virtual Database Database { get; set; }
}
