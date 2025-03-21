namespace OpenVision.Core.Features2d.Factory;

internal class AKAZEFactory : IFeatureExtractorFactory
{
    public Feature2D Create(IFeatureExtractorOptions options)
    {
#if ANDROID
        return AKAZE.Create()!;
#else
        return new AKAZE();
#endif
    }
}
