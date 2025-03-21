using System.Drawing;

namespace OpenVision.Core.DataTypes;

/// <summary>
/// Represents the result of a target matching operation.
/// </summary>
public class TargetMatchResult
{
    #region Properties

    /// <summary>
    /// Gets the ID of the matched target.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the projected region of the matched target.
    /// </summary>
    public PointF[] ProjectedRegion { get; }

    /// <summary>
    /// Gets the size of the matched target.
    /// </summary>
    public SizeF Size { get; }

    /// <summary>
    /// Gets the X-coordinate of the center of the matched target.
    /// </summary>
    public float CenterX { get; }

    /// <summary>
    /// Gets the Y-coordinate of the center of the matched target.
    /// </summary>
    public float CenterY { get; }

    /// <summary>
    /// Gets the angle of rotation of the matched target.
    /// </summary>
    public float Angle { get; }

    /// <summary>
    /// Gets the homography array of the matched target.
    /// </summary>
    public Mat HomographyArray { get; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetMatchResult"/> class with the specified ID, projected region, center coordinates, angle, and size.
    /// </summary>
    /// <param name="id">The ID of the matched target.</param>
    /// <param name="projectedRegion">The projected region of the matched target.</param>
    /// <param name="centerX">The X-coordinate of the center of the matched target.</param>
    /// <param name="centerY">The Y-coordinate of the center of the matched target.</param>
    /// <param name="angle">The angle of rotation of the matched target.</param>
    /// <param name="size">The size of the matched target.</param>
    /// <param name="homographyArray">The homography array of the matched target.</param>
    internal TargetMatchResult(
        string id,
        PointF[] projectedRegion,
        float centerX,
        float centerY,
        float angle,
        SizeF size,
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