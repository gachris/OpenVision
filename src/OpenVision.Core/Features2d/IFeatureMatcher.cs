using OpenVision.Core.DataTypes;

namespace OpenVision.Core.Features2d;

/// <summary>
/// Interface for feature matching algorithms.
/// </summary>
public interface IFeatureMatcher
{
    /// <summary>
    /// Matches features between a query image and a training image.
    /// </summary>
    /// <param name="queryInfo">Information about the query image and its features.</param>
    /// <param name="trainInfo">Information about the training image and its features.</param>
    /// <returns>A <see cref="HomographyResult"/> containing the homography matrix and other matching information.</returns>
    HomographyResult Match(TargetMatchQuery queryInfo, TargetMatchQuery trainInfo);
}