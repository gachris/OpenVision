namespace OpenVision.Core.Configuration;

/// <summary>
/// Configuration options for the KAZE feature extractor.
/// </summary>
public class KAZEOptions : IFeatureExtractorOptions
{
    /// <summary>
    /// Enum defining the diffusivity types for KAZE.
    /// </summary>
    public enum DiffusivityType
    {
        /// <summary>
        /// PM G1 diffusivity type.
        /// </summary>
        PmG1,

        /// <summary>
        /// PM G2 diffusivity type.
        /// </summary>
        PmG2,

        /// <summary>
        /// Weickert diffusivity type.
        /// </summary>
        Weickert,

        /// <summary>
        /// Charbonnier diffusivity type.
        /// </summary>
        Charbonnier
    }

    private static readonly string _name = "KAZE";
    private static readonly FeatureExtractorType _type = FeatureExtractorType.KAZE;

    /// <summary>
    /// Default KAZE options.
    /// </summary>
    public static readonly KAZEOptions Default = new()
    {
        Extended = false,
        Upright = false,
        Threshold = 0.001f,
        Octaves = 4,
        Sublevels = 4,
        Diffusivity = DiffusivityType.PmG2,
    };

    /// <inheritdoc />
    public string Name => _name;

    /// <inheritdoc />
    public FeatureExtractorType Type => _type;

    /// <summary>
    /// Indicates whether to use extended descriptor (128 elements) or not (64 elements).
    /// </summary>
    public bool Extended { get; set; }

    /// <summary>
    /// Indicates whether to force the orientation of the keypoints to be upright.
    /// </summary>
    public bool Upright { get; set; }

    /// <summary>
    /// Threshold for the feature detector response.
    /// </summary>
    public float Threshold { get; set; }

    /// <summary>
    /// Number of octaves.
    /// </summary>
    public int Octaves { get; set; }

    /// <summary>
    /// Number of sublevels per octave.
    /// </summary>
    public int Sublevels { get; set; }

    /// <summary>
    /// Diffusivity type used by the KAZE descriptor.
    /// </summary>
    public DiffusivityType Diffusivity { get; set; }
}