namespace OpenVision.Core.DataTypes;

/// <summary>
/// Represents the result of a target detection operation.
/// </summary>
public class TargetDetectionResult
{
    #region Properties

    /// <summary>
    /// Gets the input mat of target for the detection operation.
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

    internal TargetDetectionResult(
        Mat mat,
        MatOfKeyPoint keypoints,
        Mat descriptors)
    {
        Mat = mat;
        Keypoints = keypoints;
        Descriptors = descriptors;
    }
}