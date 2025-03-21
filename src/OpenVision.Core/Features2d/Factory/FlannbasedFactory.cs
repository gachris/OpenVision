using OpenVision.Core.Configuration;

namespace OpenVision.Core.Features2d.Factory;

internal class FlannbasedFactory : IFeatureMatcherFactory
{
    public DescriptorMatcher Create(IFeatureMatcherOptions options)
    {
#if ANDROID
        return FlannBasedMatcher.Create()!;
#else
        var indexParams = new Emgu.CV.Flann.LinearIndexParams();
        var searchParams = new Emgu.CV.Flann.SearchParams();
        return new FlannBasedMatcher(indexParams, searchParams);
#endif
    }
}
