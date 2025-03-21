namespace OpenVision.Core.Configuration;

/// <summary>
/// Options class for configuring the AGAST (Adaptive and Generic Accelerated Segment Test) feature detector.
/// </summary>
public class AgastFeatureDetectorOptions : IFeatureExtractorOptions
{
    private static readonly string _name = "AgastFeatureDetector";
    private static readonly FeatureExtractorType _type = FeatureExtractorType.AgastFeatureDetector;

    /// <summary>
    /// Gets the name of the AGAST feature detector.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the type of the AGAST feature detector.
    /// </summary>
    public FeatureExtractorType Type => _type;
}