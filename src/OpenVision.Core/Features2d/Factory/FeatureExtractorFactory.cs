using OpenVision.Core.Configuration;

namespace OpenVision.Core.Features2d.Factory;

internal class FeatureExtractorFactory
{
    private static readonly Dictionary<FeatureExtractorType, Type> _featureExtractorFactories = new()
    {
        { FeatureExtractorType.SIFT, typeof(SIFTFactory) },
        { FeatureExtractorType.ORB, typeof(ORBFactory) },
        { FeatureExtractorType.KAZE, typeof(KAZEFactory) },
        { FeatureExtractorType.AKAZE, typeof(AKAZEFactory) },
        { FeatureExtractorType.MSER, typeof(MSERFactory) },
        { FeatureExtractorType.AgastFeatureDetector, typeof(AgastFeatureDetectorFactory) },
        { FeatureExtractorType.BRISK, typeof(BRISKFactory) },
        { FeatureExtractorType.GFTTDetector, typeof(GFTTDetectorFactory) },
        { FeatureExtractorType.FastFeatureDetector, typeof(FastFeatureDetectorFactory) },
        { FeatureExtractorType.SimpleBlobDetector, typeof(SimpleBlobDetectorFactory) }
    };

    /// <summary>
    /// Creates a new instance of the factory for the specified feature extractor type.
    /// </summary>
    /// <param name="featureExtractorType">The type of feature extractor.</param>
    /// <returns>A new instance of the appropriate feature extractor factory.</returns>
    public static IFeatureExtractorFactory Create(FeatureExtractorType featureExtractorType)
    {
        if (_featureExtractorFactories.TryGetValue(featureExtractorType, out var factoryType))
        {
            return (IFeatureExtractorFactory)Activator.CreateInstance(factoryType)!;
        }

        throw new ArgumentOutOfRangeException(nameof(featureExtractorType), "Unsupported feature extractor type.");
    }
}
