using OpenVision.Shared.Types;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents the data used to update a target, with each field being optional.
/// </summary>
public record UpdateTargetDto
{
    /// <summary>
    /// Gets the name of the target.
    /// </summary>
    public virtual string? Name { get; init; }

    /// <summary>
    /// Gets the width of the target, in meters.
    /// </summary>
    public virtual float? Width { get; init; }

    /// <summary>
    /// Gets the image data for the target. The image must be in JPG or PNG format.
    /// </summary>
    public virtual byte[]? Image { get; init; }

    /// <summary>
    /// Gets a value indicating whether the target is active or inactive.
    /// </summary>
    public virtual ActiveFlag? ActiveFlag { get; init; }

    /// <summary>
    /// Gets the metadata for the target.
    /// </summary>
    public virtual string? Metadata { get; init; }
}