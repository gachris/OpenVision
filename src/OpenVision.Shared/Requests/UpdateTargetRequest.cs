using System.ComponentModel.DataAnnotations;
using OpenVision.Shared.Types;

namespace OpenVision.Shared.Requests;

/// <summary>
/// Represents a request to update an existing target.
/// </summary>
public record UpdateTargetRequest
{
    /// <summary>
    /// Gets or sets the name of the target.
    /// </summary>
    public virtual string? Name { get; init; }

    /// <summary>
    /// Gets or sets the width of the target, in meters.
    /// </summary>
    [Range(0.1f, float.MaxValue, ErrorMessage = "width must be greater than 0.1.")]
    public virtual float? Width { get; init; }

    /// <summary>
    /// Gets or sets the image data for the target. The image must be in JPG or PNG format.
    /// </summary>
    public virtual byte[]? Image { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the target is active or inactive.
    /// </summary>
    public virtual ActiveFlag? ActiveFlag { get; init; }

    /// <summary>
    /// Gets or sets the metadata for the target.
    /// </summary>
    public virtual string? Metadata { get; init; }
}