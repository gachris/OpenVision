namespace OpenVision.Core.Configuration;

/// <summary>
/// Options class for configuring the Brute-Force (BF) Matcher.
/// </summary>
public class BFMatcherOptions : IFeatureMatcherOptions
{
    /// <summary>
    /// Gets the name of the Brute-Force (BF) Matcher.
    /// </summary>
    public string Name => "BFMatcher";

    /// <summary>
    /// Gets the type of the Brute-Force (BF) Matcher.
    /// </summary>
    public FeatureMatcherType Type => FeatureMatcherType.BFMatcher;
}

/// <summary>
/// Options class for configuring the Brute-Force (BF) Matcher.
/// </summary>
public class FlannbasedOptions : IFeatureMatcherOptions
{
    /// <summary>
    /// Gets the name of the Brute-Force (BF) Matcher.
    /// </summary>
    public string Name => "Flannbased";

    /// <summary>
    /// Gets the type of the Brute-Force (BF) Matcher.
    /// </summary>
    public FeatureMatcherType Type => FeatureMatcherType.Flannbased;
}
