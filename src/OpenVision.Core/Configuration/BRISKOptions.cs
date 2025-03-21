namespace OpenVision.Core.Configuration;

/// <summary>
/// Options class for configuring the BRISK feature extractor.
/// </summary>
public class BRISKOptions : IFeatureExtractorOptions
{
    private static readonly string _name = "BRISK";
    private static readonly FeatureExtractorType _type = FeatureExtractorType.BRISK;

    /// <summary>
    /// Default configuration for BRISK feature extractor.
    /// </summary>
    public static readonly BRISKOptions Default = new()
    {
        Thresh = 30,
        Octaves = 3,
        PatternScale = 1f,
    };

    /// <summary>
    /// Gets the name of the BRISK feature extractor.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the type of the BRISK feature extractor.
    /// </summary>
    public FeatureExtractorType Type => _type;

    /// <summary>
    /// Threshold for BRISK feature detection.
    /// </summary>
    public int Thresh { get; set; }

    /// <summary>
    /// Number of octaves for BRISK feature detection.
    /// </summary>
    public int Octaves { get; set; }

    /// <summary>
    /// Scale of the pattern used for BRISK feature detection.
    /// </summary>
    public float PatternScale { get; set; }
}
