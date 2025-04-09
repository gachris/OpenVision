using OpenVision.Shared;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents a request to update a trackable.
/// </summary>
public class UpdateTrackableDto
{
    /// <summary>
    /// Gets or sets the name of the trackable.
    /// </summary>
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the width of the trackable, in meters.
    /// </summary>
    public virtual float? Width { get; set; }

    /// <summary>
    /// Gets or sets the URL of the image to use for the trackable.
    /// </summary>
    public virtual string? Image { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the trackable is active or inactive.
    /// </summary>
    public virtual ActiveFlag? ActiveFlag { get; set; }

    /// <summary>
    /// Gets or sets the metadata for the trackable.
    /// </summary>
    public virtual string? Metadata { get; set; }
}