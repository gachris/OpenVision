using OpenVision.Core.Configuration;

namespace OpenVision.Core.Features2d.Factory;

internal class AgastFeatureDetectorFactory : IFeatureExtractorFactory
{
    public Feature2D Create(IFeatureExtractorOptions options)
    {
#if ANDROID
        return AgastFeatureDetector.Create()!;
#else
        return new AgastFeatureDetector();
#endif
    }
}