using OpenVision.Core.Features2d.Factory;

namespace OpenVision.Core.Features2d;

/// <summary>
/// Feature extractor implementation that detects and computes features in images.
/// </summary>
public class FeatureExtractor : IFeatureExtractor
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureExtractor"/> class.
    /// </summary>
    public FeatureExtractor()
    {
    }

    #region IFeaturesDetector Implementation

    /// <inheritdoc/>
    public TargetDetectionResult DetectAndCompute(IImageRequest request)
    {
        using var featureDetector = CreateFeature2D();
        var keypoints = new MatOfKeyPoint();
        var descriptors = new Mat();
        var mask = new Mat();

        featureDetector.DetectAndCompute(request.Mat, mask, keypoints, descriptors, false);

        return new TargetDetectionResult(
            request.Mat,
            keypoints,
            descriptors);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a specific type of feature detector based on the configured options.
    /// </summary>
    /// <returns>A new instance of the configured feature detector.</returns>
    private static Feature2D CreateFeature2D()
    {
        var featureExtractorFactory = FeatureExtractorFactory.Create(VisionSystemConfig.FeatureExtractorOptions.Type);
        return featureExtractorFactory.Create(VisionSystemConfig.FeatureExtractorOptions);
    }

    #endregion
}