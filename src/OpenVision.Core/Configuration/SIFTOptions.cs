namespace OpenVision.Core.Configuration;

/// <summary>
/// Options for configuring the SIFT (Scale-Invariant Feature Transform) feature extractor.
/// </summary>
public class SIFTOptions : IFeatureExtractorOptions
{
    private static readonly string _name = "SIFT";
    private static readonly FeatureExtractorType _type = FeatureExtractorType.SIFT;

    /// <summary>
    /// Default SIFT options.
    /// </summary>
    public static readonly SIFTOptions Default = new()
    {
        NumFeatures = 0,
        NumOctaveLayers = 3,
        ContrastThreshold = 0.04,
        EdgeThreshold = 10.0,
        Sigma = 1.6
    };

    /// <summary>
    /// Gets the name of the feature extractor.
    /// </summary>
    public string Name => _name;

    /// <summary>
    /// Gets the type of the feature extractor.
    /// </summary>
    public FeatureExtractorType Type => _type;

    /// <summary>
    /// The maximum number of keypoints to be detected in an image.
    /// </summary>
    public int NumFeatures { get; set; }

    /// <summary>
    /// Number of scales per octave.
    /// </summary>
    public int NumOctaveLayers { get; set; }

    /// <summary>
    /// Contrast threshold for keypoints.
    /// </summary>
    public double ContrastThreshold { get; set; }

    /// <summary>
    /// Edge threshold for keypoints.
    /// </summary>
    public double EdgeThreshold { get; set; }

    /// <summary>
    /// Sigma value for Gaussian blur.
    /// </summary>
    public double Sigma { get; set; }
}
