using OpenVision.Core.Configuration;

namespace OpenVision.Core.Features2d.Factory;

internal class SimpleBlobDetectorFactory : IFeatureExtractorFactory
{
    public Feature2D Create(IFeatureExtractorOptions options)
    {
#if ANDROID
        return SimpleBlobDetector.Create()!;
#else
        return new SimpleBlobDetector();
#endif
    }
}