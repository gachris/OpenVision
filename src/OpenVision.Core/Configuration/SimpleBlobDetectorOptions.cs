namespace OpenVision.Core.Configuration;

/// <summary>
/// Options for configuring the SimpleBlobDetector feature extractor.
/// </summary>
public class SimpleBlobDetectorOptions : IFeatureExtractorOptions
{
    private static readonly string _name = "SimpleBlobDetector";
    private static readonly FeatureExtractorType _type = FeatureExtractorType.SimpleBlobDetector;

    /// <summary>
    /// Gets the name of the feature extractor.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the type of the feature extractor.
    /// </summary>
    public FeatureExtractorType Type => _type;
}