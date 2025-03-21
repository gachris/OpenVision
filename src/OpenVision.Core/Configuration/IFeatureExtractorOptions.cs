namespace OpenVision.Core.Configuration;

/// <summary>
/// Represents options for configuring a feature extractor.
/// </summary>
public interface IFeatureExtractorOptions
{
    /// <summary>
    /// Gets the name of the feature extractor.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the type of the feature extractor.
    /// </summary>
    FeatureExtractorType Type { get; }
}
