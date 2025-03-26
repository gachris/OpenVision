using System.Drawing;
using System.Text.Json.Serialization;
using OpenVision.Core.Reco.Json.Converters;

namespace OpenVision.Core.Reco.DataTypes.Responses;

/// <summary>
/// Represents the response of a target matching operation.
/// </summary>
public class TargetMatchResponse
{
    /// <summary>
    /// Gets the ID of the matched target.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; }

    /// <summary>
    /// Gets the array of projected region points of the matched target.
    /// </summary>
    [JsonPropertyName("projected_region")]
    public PointF[] ProjectedRegion { get; }

    /// <summary>
    /// Gets the size of the matched target.
    /// </summary>
    [JsonPropertyName("size")]
    public SizeF Size { get; }

    /// <summary>
    /// Gets the X-coordinate of the center of the matched target.
    /// </summary>
    [JsonPropertyName("center_x")]
    public float CenterX { get; }

    /// <summary>
    /// Gets the Y-coordinate of the center of the matched target.
    /// </summary>
    [JsonPropertyName("center_y")]
    public float CenterY { get; }

    /// <summary>
    /// Gets the angle of the matched target.
    /// </summary>
    [JsonPropertyName("angle")]
    public float Angle { get; }

    /// <summary>
    /// Gets the homography matrix array.
    /// </summary>
    [JsonPropertyName("homography")]
    [JsonConverter(typeof(MatConverter))]
    public Mat HomographyArray { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetMatchResponse"/> class with the specified values.
    /// </summary>
    /// <param name="id">The ID of the matched target.</param>
    /// <param name="projectedRegion">The array of projected region points of the matched target.</param>
    /// <param name="size">The size of the matched target.</param>
    /// <param name="centerX">The X-coordinate of the center of the matched target.</param>
    /// <param name="centerY">The Y-coordinate of the center of the matched target.</param>
    /// <param name="angle">The angle of the matched target.</param>
    /// <param name="homographyArray">The homography matrix array.</param>
    public TargetMatchResponse(
        string id,
        PointF[] projectedRegion,
        SizeF size,
        float centerX,
        float centerY,
        float angle,
        Mat homographyArray)
    {
        Id = id;
        ProjectedRegion = projectedRegion;
        CenterX = centerX;
        CenterY = centerY;
        Angle = angle;
        Size = size;
        HomographyArray = homographyArray;
    }
}
