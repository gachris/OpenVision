using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OpenVision.Shared.Requests;

/// <summary>
/// Represents a request to create a new target.
/// </summary>
public class PostTargetRequest
{
    /// <summary>
    /// Gets or sets the name of the target.
    /// </summary>
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Target name is required.")]
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the ID of the database that the target belongs to.
    /// </summary>
    [JsonPropertyName("database_id")]
    [Required(ErrorMessage = "Database id is required.")]
    public virtual Guid? DatabaseId { get; set; }

    /// <summary>
    /// Gets or sets the image data for the target. The image must be in JPG or PNG format.
    /// </summary>
    [JsonPropertyName("image")]
    [Required(ErrorMessage = "Image is required. The image must be jpg or png.")]
    public virtual byte[]? Image { get; set; }

    /// <summary>
    /// Gets or sets the size of the target in the X dimension.
    /// </summary>
    [JsonPropertyName("width")]
    [Required(ErrorMessage = "Target width is required.")]
    [Range(0.1f, float.MaxValue, ErrorMessage = "width must be greater than 0.1.")]
    public virtual float? Width { get; set; }

    /// <summary>
    /// Gets or sets the type of the target.
    /// </summary>
    [JsonPropertyName("type")]
    [Required(ErrorMessage = "Target type is required.")]
    public virtual TargetType? Type { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the target is active or inactive.
    /// </summary>
    [JsonPropertyName("active_flag")]
    public virtual ActiveFlag? ActiveFlag { get; set; }

    /// <summary>
    /// Gets or sets the metadata for the target.
    /// </summary>
    [JsonPropertyName("metadata")]
    public virtual string? Metadata { get; set; }
}