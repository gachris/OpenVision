using OpenVision.Shared;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents the data used to update a target, with each field being optional.
/// </summary>
public class UpdateTargetDto
{
    /// <summary>
    /// Gets or sets the name of the target.
    /// </summary>
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the width of the target, in meters.
    /// </summary>
    public virtual float? Width { get; set; }

    /// <summary>
    /// Gets or sets the image data for the target. The image must be in JPG or PNG format.
    /// </summary>
    public virtual byte[]? Image { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the target is active or inactive.
    /// </summary>
    public virtual ActiveFlag? ActiveFlag { get; set; }

    /// <summary>
    /// Gets or sets the metadata for the target.
    /// </summary>
    public virtual string? Metadata { get; set; }
}