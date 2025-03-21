namespace OpenVision.Core.DataTypes;

/// <summary>
/// Represents a query for target matching.
/// </summary>
public class TargetMatchQuery
{
    #region Properties

    /// <summary>
    /// Gets the ID for the detection operation.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the input Mat of the target for the detection operation.
    /// </summary>
    public Mat Mat { get; }

    /// <summary>
    /// Gets the detected keypoints in the input target.
    /// </summary>
    public MatOfKeyPoint Keypoints { get; }

    /// <summary>
    /// Gets the descriptors of the detected keypoints.
    /// </summary>
    public Mat Descriptors { get; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetMatchQuery"/> class.
    /// </summary>
    /// <param name="id">The ID for the detection operation.</param>
    /// <param name="mat">The input Mat of the target.</param>
    /// <param name="keypoints">The detected keypoints in the input target.</param>
    /// <param name="descriptors">The descriptors of the detected keypoints.</param>
    internal TargetMatchQuery(
        string id,
        Mat mat,
        MatOfKeyPoint keypoints,
        Mat descriptors)
    {
        Id = id;
        Mat = mat;
        Keypoints = keypoints;
        Descriptors = descriptors;
    }
}