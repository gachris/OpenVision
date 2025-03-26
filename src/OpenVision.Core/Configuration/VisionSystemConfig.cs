namespace OpenVision.Core.Configuration;

/// <summary>
/// Provides global configuration settings for the vision system, including WebSocket URL, 
/// feature extractor options, and feature matcher options.
/// </summary>
public class VisionSystemConfig
{
    /// <summary>
    /// The WebSocket URL used for connecting to the vision cloud service.
    /// </summary>
    public static string WebSocketUrl = "wss://vision-cloud.pixel-prodigy.com/ws";

    /// <summary>
    /// The configuration options for the feature extractor, defining parameters for feature detection.
    /// </summary>
    public static IFeatureExtractorOptions FeatureExtractorOptions = new SIFTOptions
    {
        NumFeatures = 300,
        NumOctaveLayers = 3,
        ContrastThreshold = 0.04,
        EdgeThreshold = 10.0,
        Sigma = 1.6
    };

    /// <summary>
    /// The configuration options for the feature matcher, defining how keypoints and descriptors are matched.
    /// </summary>
    public static IFeatureMatcherOptions FeatureMatcherOptions = new BFMatcherOptions();

    /// <summary>
    /// The configuration options for the ImageRequestBuilder.
    /// </summary>
    public static ImageRequestBuilder ImageRequestBuilder = new ImageRequestBuilder().WithGrayscale();
}
