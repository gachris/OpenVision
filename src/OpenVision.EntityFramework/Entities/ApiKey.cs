using OpenVision.Shared.Types;

namespace OpenVision.EntityFramework.Entities;

/// <summary>
/// Represents an API key entity.
/// </summary>
public partial class ApiKey
{
    /// <summary>
    /// Gets or sets the unique identifier of the API key.
    /// </summary>
    public virtual required Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated database.
    /// </summary>
    public virtual required Guid DatabaseId { get; set; }

    /// <summary>
    /// Gets or sets the API key value.
    /// </summary>
    public virtual required string Key { get; set; }

    /// <summary>
    /// Gets or sets the type of the API key.
    /// </summary>
    public virtual required ApiKeyType Type { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the API key was created.
    /// </summary>
    public virtual required DateTimeOffset Created { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the API key was last updated.
    /// </summary>
    public virtual required DateTimeOffset Updated { get; set; }

    /// <summary>
    /// Gets or sets the associated database entity navigation property.
    /// </summary>
    public virtual Database? Database { get; set; }
}
