using OpenVision.Shared.Types;

namespace OpenVision.Server.Core.Dtos;

/// <summary>
/// Represents the data required to create a new target.
/// </summary>
public record CreateTargetDto
{
    /// <summary>
    /// Gets the name of the target.
    /// </summary>
    public required virtual string Name { get; init; }

    /// <summary>
    /// Gets the ID of the database that the target belongs to.
    /// </summary>
    public virtual Guid? DatabaseId { get; init; }

    /// <summary>
    /// Gets the image data for the target. The image must be in JPG or PNG format.
    /// </summary>
    public required virtual byte[] Image { get; init; }

    /// <summary>
    /// Gets the size of the target in the X dimension.
    /// </summary>
    public required virtual float Width { get; init; }

    /// <summary>
    /// Gets the type of the target.
    /// </summary>
    public virtual TargetType Type { get; init; }

    /// <summary>
    /// Gets a value indicating whether the target is active or inactive.
    /// </summary>
    public virtual ActiveFlag? ActiveFlag { get; init; }

    /// <summary>
    /// Gets the metadata for the target.
    /// </summary>
    public virtual string? Metadata { get; init; }
}