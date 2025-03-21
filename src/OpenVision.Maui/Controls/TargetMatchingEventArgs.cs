using OpenVision.Core.DataTypes;

namespace OpenVision.Maui.Controls;

/// <summary>
/// Event arguments containing information about a target matching event.
/// </summary>
public class TargetMatchingEventArgs
{
    /// <summary>
    /// Gets the camera frame.
    /// </summary>
    public Mat Frame { get; }

    /// <summary>
    /// Gets the target match results.
    /// </summary>
    public IReadOnlyCollection<TargetMatchResult> TargetMatchResults { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetMatchingEventArgs"/> class.
    /// </summary>
    /// <param name="frame">The camera frame.</param>
    /// <param name="targetMatchResults">The target match results.</param>
    internal TargetMatchingEventArgs(Mat frame, IReadOnlyCollection<TargetMatchResult> targetMatchResults)
    {
        Frame = frame;
        TargetMatchResults = targetMatchResults;
    }
}