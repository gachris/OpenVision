namespace OpenVision.Core.Configuration;

/// <summary>
/// Options for configuring the MSER (Maximally Stable Extremal Regions) feature extractor.
/// </summary>
public class MSEROptions : IFeatureExtractorOptions
{
    private static readonly string _name = "MSER";
    private static readonly FeatureExtractorType _type = FeatureExtractorType.MSER;

    /// <summary>
    /// Gets the name of the feature extractor.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the type of the feature extractor.
    /// </summary>
    public FeatureExtractorType Type => _type;
}
