using System.ComponentModel.DataAnnotations;
using OpenVision.Shared.Types;

namespace OpenVision.Shared.Requests;

/// <summary>
/// Represents a request to create a new trackable.
/// </summary>
public record PostTrackableRequest
{
    /// <summary>
    /// Gets or sets the name of the trackable.
    /// </summary>
    [Required(ErrorMessage = "name is required.")]
    public virtual required string Name { get; init; }

    /// <summary>
    /// Gets or sets the width of the trackable, in meters.
    /// </summary>
    [Required(ErrorMessage = "width is required.")]
    [Range(0.1f, float.MaxValue, ErrorMessage = "width must be greater than 0.1.")]
    public virtual required float Width { get; init; }

    /// <summary>
    /// Gets or sets the URL of the image to use for the trackable.
    /// </summary>
    [Required(ErrorMessage = "image is required.")]
    public virtual required string Image { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the trackable is active or inactive.
    /// </summary>
    public virtual ActiveFlag? ActiveFlag { get; init; }

    /// <summary>
    /// Gets or sets the metadata for the trackable.
    /// </summary>
    public virtual string? Metadata { get; init; }
}