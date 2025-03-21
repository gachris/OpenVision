namespace OpenVision.Core.Configuration;

/// <summary>
/// Interface for feature matcher options.
/// </summary>
public interface IFeatureMatcherOptions
{
    /// <summary>
    /// Gets the name of the feature matcher.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the type of the feature matcher.
    /// </summary>
    public FeatureMatcherType Type { get; }
}
