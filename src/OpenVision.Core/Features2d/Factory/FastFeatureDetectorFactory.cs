namespace OpenVision.Core.Features2d.Factory;

internal class FastFeatureDetectorFactory : IFeatureExtractorFactory
{
    public Feature2D Create(IFeatureExtractorOptions options)
    {
#if ANDROID
        return FastFeatureDetector.Create()!;
#else
        return new FastFeatureDetector();
#endif
    }
}
