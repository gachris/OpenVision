using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OpenVision.Shared.Requests;

/// <summary>
/// Represents a request to update an existing target.
/// </summary>
public class UpdateTargetRequest
{
    /// <summary>
    /// Gets or sets the name of the target.
    /// </summary>
    [JsonPropertyName("name")]
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the width of the target, in meters.
    /// </summary>
    [JsonPropertyName("width")]
    [Range(0.1f, float.MaxValue, ErrorMessage = "width must be greater than 0.1.")]
    public virtual float? Width { get; set; }

    /// <summary>
    /// Gets or sets the image data for the target. The image must be in JPG or PNG format.
    /// </summary>
    [JsonPropertyName("image")]
    public virtual byte[]? Image { get; set; }

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