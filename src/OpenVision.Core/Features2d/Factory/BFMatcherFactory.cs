using OpenVision.Core.Configuration;

namespace OpenVision.Core.Features2d.Factory;

internal class BFMatcherFactory : IFeatureMatcherFactory
{
    public DescriptorMatcher Create(IFeatureMatcherOptions options)
    {
        var bFMatcherOptions = (BFMatcherOptions)VisionSystemConfig.FeatureMatcherOptions;
#if ANDROID
        return DescriptorMatcher.Create(DescriptorMatcher.Flannbased)!;
#else
        return new BFMatcher(DistanceType.L2);
#endif
    }
}