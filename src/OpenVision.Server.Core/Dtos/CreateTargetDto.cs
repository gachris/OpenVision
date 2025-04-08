using OpenVision.Shared;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents the data required to create a new target.
/// </summary>
public class CreateTargetDto
{
    /// <summary>
    /// Gets or sets the name of the target.
    /// </summary>
    public required virtual string Name { get; set; }

    /// <summary>
    /// Gets or sets the ID of the database that the target belongs to.
    /// </summary>
    public required virtual Guid DatabaseId { get; set; }

    /// <summary>
    /// Gets or sets the image data for the target. The image must be in JPG or PNG format.
    /// </summary>
    public required virtual byte[] Image { get; set; }

    /// <summary>
    /// Gets or sets the size of the target in the X dimension.
    /// </summary>
    public required virtual float Width { get; set; }

    /// <summary>
    /// Gets or sets the type of the target.
    /// </summary>
    public required virtual TargetType Type { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the target is active or inactive.
    /// </summary>
    public virtual ActiveFlag? ActiveFlag { get; set; }

    /// <summary>
    /// Gets or sets the metadata for the target.
    /// </summary>
    public virtual string? Metadata { get; set; }
}