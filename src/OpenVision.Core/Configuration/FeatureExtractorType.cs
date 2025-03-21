namespace OpenVision.Core.Configuration;

/// <summary>
/// Enumeration representing different types of feature extractors and detectors used in computer vision.
/// </summary>
public enum FeatureExtractorType
{
    /// <summary>
    /// Scale-Invariant Feature Transform (SIFT) feature extractor.
    /// </summary>
    SIFT,

    /// <summary>
    /// Oriented FAST and Rotated BRIEF (ORB) feature extractor.
    /// </summary>
    ORB,

    /// <summary>
    /// KAZE feature extractor, also known as Accelerated-KAZE.
    /// </summary>
    KAZE,

    /// <summary>
    /// Accelerated-KAZE (AKAZE) feature extractor.
    /// </summary>
    AKAZE,

    /// <summary>
    /// Maximally Stable Extremal Regions (MSER) feature detector.
    /// </summary>
    MSER,

    /// <summary>
    /// Binary Robust Invariant Scalable Keypoints (BRISK) feature extractor.
    /// </summary>
    BRISK,

    /// <summary>
    /// Good Features to Track (GFTT) feature detector.
    /// </summary>
    GFTTDetector,

    /// <summary>
    /// AGAST (Adaptive and Generic Accelerated Segment Test) feature detector.
    /// </summary>
    AgastFeatureDetector,

    /// <summary>
    /// FAST (Features from Accelerated Segment Test) feature detector.
    /// </summary>
    FastFeatureDetector,

    /// <summary>
    /// Simple Blob Detector feature extractor.
    /// </summary>
    SimpleBlobDetector
}