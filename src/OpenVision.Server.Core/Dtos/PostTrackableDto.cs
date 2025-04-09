using OpenVision.Shared;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents a request to create a new trackable.
/// </summary>
public class PostTrackableDto
{
    /// <summary>
    /// Gets or sets the name of the trackable.
    /// </summary>
    public required virtual string Name { get; set; }

    /// <summary>
    /// Gets or sets the width of the trackable, in meters.
    /// </summary>
    public required virtual float Width { get; set; }

    /// <summary>
    /// Gets or sets the URL of the image to use for the trackable.
    /// </summary>
    public required virtual string Image { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the trackable is active or inactive.
    /// </summary>
    public virtual ActiveFlag? ActiveFlag { get; set; }

    /// <summary>
    /// Gets or sets the metadata for the trackable.
    /// </summary>
    public virtual string? Metadata { get; set; }
}