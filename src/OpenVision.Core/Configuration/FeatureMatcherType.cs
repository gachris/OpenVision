namespace OpenVision.Core.Configuration;

/// <summary>
/// Types of feature matchers.
/// </summary>
public enum FeatureMatcherType
{
    /// <summary>
    /// Brute-force feature matcher.
    /// </summary>
    BFMatcher,

    /// <summary>
    /// FLANN-based (Fast Library for Approximate Nearest Neighbors) feature matcher.
    /// </summary>
    Flannbased
}