namespace OpenVision.Core.Configuration;

/// <summary>
/// Configuration options for the Good Features to Track (GFTT) detector.
/// </summary>
public class GFTTDetectorOptions : IFeatureExtractorOptions
{
    private static readonly string _name = "GFTTDetector";
    private static readonly FeatureExtractorType _type = FeatureExtractorType.GFTTDetector;

    /// <summary>
    /// Default GFTT detector options.
    /// </summary>
    public static readonly GFTTDetectorOptions Default = new()
    {
        MaxCorners = 1000,
        QualityLevel = 0.01,
        MinDistance = 1.0,
        BlockSize = 3,
        UseHarrisDetector = false,
        K = 0.04
    };

    /// <inheritdoc />
    public string Name => _name;

    /// <inheritdoc />
    public FeatureExtractorType Type => _type;

    /// <summary>
    /// Maximum number of corners to return.
    /// </summary>
    public int MaxCorners { get; set; }

    /// <summary>
    /// Parameter characterizing the minimal accepted quality of image corners.
    /// </summary>
    public double QualityLevel { get; set; }

    /// <summary>
    /// Minimum possible Euclidean distance between the returned corners.
    /// </summary>
    public double MinDistance { get; set; }

    /// <summary>
    /// Size of the averaging block for computing derivative covariation matrix over each pixel neighborhood.
    /// </summary>
    public int BlockSize { get; set; }

    /// <summary>
    /// Flag indicating whether to use Harris detector (true) or cornerMinEigenVal (false).
    /// </summary>
    public bool UseHarrisDetector { get; set; }

    /// <summary>
    /// Free parameter of the Harris detector.
    /// </summary>
    public double K { get; set; }
}
