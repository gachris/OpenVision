namespace OpenVision.Core.Configuration;

/// <summary>
/// Options class for configuring the Fast Feature Detector.
/// </summary>
public class FastFeatureDetectorOptions : IFeatureExtractorOptions
{
    private static readonly string _name = "FastFeatureDetector";
    private static readonly FeatureExtractorType _type = FeatureExtractorType.FastFeatureDetector;

    /// <summary>
    /// Gets the name of the Fast Feature Detector.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the type of the Fast Feature Detector.
    /// </summary>
    public FeatureExtractorType Type => _type;
}
