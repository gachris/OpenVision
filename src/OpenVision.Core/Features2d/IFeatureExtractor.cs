namespace OpenVision.Core.Features2d;

/// <summary>
/// Interface for feature extraction from images.
/// </summary>
public interface IFeatureExtractor
{
    /// <summary>
    /// Detects and computes features in the provided image request.
    /// </summary>
    /// <param name="detectAndComputeRequest">The request containing image data and parameters.</param>
    /// <returns>A <see cref="TargetDetectionResult"/> containing detected keypoints and descriptors.</returns>
    TargetDetectionResult DetectAndCompute(IImageRequest detectAndComputeRequest);
}
