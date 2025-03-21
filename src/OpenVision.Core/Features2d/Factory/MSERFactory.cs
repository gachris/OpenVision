using OpenVision.Core.Configuration;

namespace OpenVision.Core.Features2d.Factory;

internal class MSERFactory : IFeatureExtractorFactory
{
    public Feature2D Create(IFeatureExtractorOptions options)
    {
#if ANDROID
        return MSER.Create()!;
#else
        return new MSER();
#endif
    }
}
