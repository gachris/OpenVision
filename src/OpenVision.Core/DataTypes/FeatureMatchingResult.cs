namespace OpenVision.Core.DataTypes;

/// <summary>
/// Represents the result of a feature matching operation.
/// </summary>
public sealed class FeatureMatchingResult
{
    #region Properties

    /// <summary>
    /// Gets a value indicating whether the result contains any matches.
    /// </summary>
    public bool HasMatches { get; }

    /// <summary>
    /// Gets a read-only collection of target match results.
    /// </summary>
    public IReadOnlyCollection<TargetMatchResult> Matches { get; }

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureMatchingResult"/> class with the specified collection of target match results.
    /// </summary>
    /// <param name="matches">The collection of target match results.</param>
    internal FeatureMatchingResult(IReadOnlyCollection<TargetMatchResult> matches)
    {
        Matches = matches;
        HasMatches = matches.Count != 0;
    }
}