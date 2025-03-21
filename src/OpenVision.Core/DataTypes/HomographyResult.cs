using OpenVision.Core.Utils;

namespace OpenVision.Core.DataTypes;

/// <summary>
/// Represents the result of a homography computation.
/// </summary>
public class HomographyResult
{
    #region Properties

    /// <summary>
    /// Gets the homography matrix associated with this result.
    /// </summary>
    public Mat? Homography { get; }

    /// <summary>
    /// Gets a value indicating whether a match was found during the homography computation.
    /// </summary>
    public bool MatchFound { get; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="HomographyResult"/> class with the specified homography matrix.
    /// </summary>
    /// <param name="homography">The homography matrix associated with this result.</param>
    internal HomographyResult(Mat? homography)
    {
        Homography = homography;
        MatchFound = Homography is not null && !Homography.IsEmpty();
    }
}
