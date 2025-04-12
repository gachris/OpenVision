using System.ComponentModel.DataAnnotations;
using OpenVision.Shared.Types;

namespace OpenVision.Shared.Requests;

/// <summary>
/// Represents a request to update a trackable.
/// </summary>
public record UpdateTrackableRequest
{
    /// <summary>
    /// Gets or sets the name of the trackable.
    /// </summary>
    public virtual string? Name { get; init; }

    /// <summary>
    /// Gets or sets the width of the trackable, in meters.
    /// </summary>
    [Range(0.1f, float.MaxValue, ErrorMessage = "width must be greater than 0.1.")]
    public virtual float? Width { get; init; }

    /// <summary>
    /// Gets or sets the URL of the image to use for the trackable.
    /// </summary>
    public virtual string? Image { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the trackable is active or inactive.
    /// </summary>
    public virtual ActiveFlag? ActiveFlag { get; init; }

    /// <summary>
    /// Gets or sets the metadata for the trackable.
    /// </summary>
    public virtual string? Metadata { get; init; }
}