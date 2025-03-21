namespace OpenVision.Core.Configuration;

/// <summary>
/// Options class for configuring the AKAZE (Accelerated-KAZE) feature extractor.
/// </summary>
public class AKAZEOptions : IFeatureExtractorOptions
{
    private static readonly string _name = "AKAZE";
    private static readonly FeatureExtractorType _type = FeatureExtractorType.AKAZE;

    /// <summary>
    /// Gets the name of the AKAZE feature extractor.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the type of the AKAZE feature extractor.
    /// </summary>
    public FeatureExtractorType Type => _type;
}
