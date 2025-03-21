using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OpenVision.Shared.Requests;

/// <summary>
/// Represents a request to update a trackable.
/// </summary>
public class UpdateTrackableRequest
{
    /// <summary>
    /// Gets or sets the name of the trackable.
    /// </summary>
    [JsonPropertyName("name")]
    public virtual string? Name { get; set; }

    /// <summary>
    /// Gets or sets the width of the trackable, in meters.
    /// </summary>
    [JsonPropertyName("width")]
    [Range(0.1f, float.MaxValue, ErrorMessage = "width must be greater than 0.1.")]
    public virtual float? Width { get; set; }

    /// <summary>
    /// Gets or sets the URL of the image to use for the trackable.
    /// </summary>
    [JsonPropertyName("image")]
    public virtual string? Image { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the trackable is active or inactive.
    /// </summary>
    [JsonPropertyName("active_flag")]
    public virtual ActiveFlag? ActiveFlag { get; set; }

    /// <summary>
    /// Gets or sets the metadata for the trackable.
    /// </summary>
    [JsonPropertyName("metadata")]
    public virtual string? Metadata { get; set; }
}