namespace OpenVision.Core.Configuration;

/// <summary>
/// Options for configuring the ORB (Oriented FAST and Rotated BRIEF) feature extractor.
/// </summary>
public class ORBOptions : IFeatureExtractorOptions
{
    /// <summary>
    /// The score type for ORB feature detector.
    /// </summary>
    public enum ScoreType
    {
        /// <summary>
        /// Harris score.
        /// </summary>
        Harris,

        /// <summary>
        /// FAST score.
        /// </summary>
        Fast
    }

    private static readonly string _name = "ORB";
    private static readonly FeatureExtractorType _type = FeatureExtractorType.ORB;

    /// <summary>
    /// Default ORB options.
    /// </summary>
    public static readonly ORBOptions Default = new()
    {
        NumberOfFeatures = 500,
        ScaleFactor = 1.2f,
        NumLevels = 8,
        EdgeThreshold = 31,
        FirstLevel = 0,
        WTK_A = 2,
        Score = ScoreType.Harris,
        PatchSize = 31,
        FastThreshold = 20,
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
    /// The maximum number of features to be detected.
    /// </summary>
    public int NumberOfFeatures { get; set; }

    /// <summary>
    /// Pyramid decimation ratio, greater than 1.
    /// </summary>
    public float ScaleFactor { get; set; }

    /// <summary>
    /// Number of pyramid levels.
    /// </summary>
    public int NumLevels { get; set; }

    /// <summary>
    /// Edge threshold. This is used to filter out weak features.
    /// </summary>
    public int EdgeThreshold { get; set; }

    /// <summary>
    /// The first level to start feature extraction.
    /// </summary>
    public int FirstLevel { get; set; }

    /// <summary>
    /// The WTA_K parameter used in the BRIEF descriptor calculation.
    /// </summary>
    public int WTK_A { get; set; }

    /// <summary>
    /// Score type used in feature detection (Harris or FAST).
    /// </summary>
    public ScoreType Score { get; set; }

    /// <summary>
    /// Size of the patch used by the oriented BRIEF descriptor.
    /// </summary>
    public int PatchSize { get; set; }

    /// <summary>
    /// Threshold on the intensity difference to accept a point as a corner.
    /// </summary>
    public int FastThreshold { get; set; }
}
