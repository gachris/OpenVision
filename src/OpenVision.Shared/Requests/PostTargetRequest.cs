using System.ComponentModel.DataAnnotations;
using OpenVision.Shared.Types;

namespace OpenVision.Shared.Requests;

/// <summary>
/// Represents a request to create a new target.
/// </summary>
public record PostTargetRequest
{
    /// <summary>
    /// Gets or sets the name of the target.
    /// </summary>
    [Required(ErrorMessage = "Target name is required.")]
    public virtual required string Name { get; init; }

    /// <summary>
    /// Gets or sets the ID of the database that the target belongs to.
    /// </summary>
    [Required(ErrorMessage = "Database id is required.")]
    public virtual required Guid DatabaseId { get; init; }

    /// <summary>
    /// Gets or sets the image data for the target. The image must be in JPG or PNG format.
    /// </summary>
    [Required(ErrorMessage = "Image is required. The image must be jpg or png.")]
    public virtual required byte[] Image { get; init; }

    /// <summary>
    /// Gets or sets the size of the target in the X dimension.
    /// </summary>
    [Required(ErrorMessage = "Target width is required.")]
    [Range(0.1f, float.MaxValue, ErrorMessage = "width must be greater than 0.1.")]
    public virtual required float Width { get; init; }

    /// <summary>
    /// Gets or sets the type of the target.
    /// </summary>
    [Required(ErrorMessage = "Target type is required.")]
    public virtual required TargetType Type { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether the target is active or inactive.
    /// </summary>
    public virtual ActiveFlag? ActiveFlag { get; init; }

    /// <summary>
    /// Gets or sets the metadata for the target.
    /// </summary>
    public virtual string? Metadata { get; init; }
}