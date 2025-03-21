using OpenVision.Core.Reco.Json.Converters;
using System.Text.Json.Serialization;

namespace OpenVision.Core.Reco.DataTypes.Requests;

/// <summary>
/// Represents a match request for a web socket.
/// </summary>
public class WSMatchRequest
{
    /// <summary>
    /// Gets the ID associated with this match request.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; }

    /// <summary>
    /// Gets the image associated with this match request.
    /// </summary>
    [JsonPropertyName("mat")]
    [JsonConverter(typeof(MatConverter))]
    public Mat Mat { get; }

    /// <summary>
    /// Gets the original width of the image associated with this match request.
    /// </summary>
    [JsonPropertyName("original_width")]
    public int OriginalWidth { get; }

    /// <summary>
    /// Gets the original height of the image associated with this match request.
    /// </summary>
    [JsonPropertyName("original_height")]
    public int OriginalHeight { get; }

    /// <summary>
    /// Gets a value indicating whether the image associated with this match request is grayscale.
    /// </summary>
    [JsonPropertyName("is_grayscale")]
    public bool IsGrayscale { get; }

    /// <summary>
    /// Gets a value indicating whether the image associated with this match request is low resolution.
    /// </summary>
    [JsonPropertyName("is_low_resolution")]
    public bool IsLowResolution { get; }

    /// <summary>
    /// Gets a value indicating whether the image associated with this match request has a region of interest (ROI).
    /// </summary>
    [JsonPropertyName("has_roi")]
    public bool HasRoi { get; }

    /// <summary>
    /// Gets a value indicating whether the image associated with this match request has Gaussian blur applied to it.
    /// </summary>
    [JsonPropertyName("has_gaussian_blur")]
    public bool HasGaussianBlur { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WSMatchRequest"/> class with the specified parameters.
    /// </summary>
    /// <param name="id">The ID associated with this match request.</param>
    /// <param name="mat">The image associated with this match request.</param>
    /// <param name="originalWidth">The original width of the image associated with this match request.</param>
    /// <param name="originalHeight">The original height of the image associated with this match request.</param>
    /// <param name="isGrayscale">A value indicating whether the image associated with this match request is grayscale.</param>
    /// <param name="isLowResolution">A value indicating whether the image associated with this match request is low resolution.</param>
    /// <param name="hasRoi">A value indicating whether the image associated with this match request has a region of interest (ROI).</param>
    /// <param name="hasGaussianBlur">A value indicating whether the image associated with this match request has Gaussian blur applied to it.</param>
    public WSMatchRequest(
        string id,
        Mat mat,
        int originalWidth,
        int originalHeight,
        bool isGrayscale,
        bool isLowResolution,
        bool hasRoi,
        bool hasGaussianBlur)
    {
        Id = id;
        Mat = mat;
        OriginalWidth = originalWidth;
        OriginalHeight = originalHeight;
        IsGrayscale = isGrayscale;
        IsLowResolution = isLowResolution;
        HasRoi = hasRoi;
        HasGaussianBlur = hasGaussianBlur;
    }
}
