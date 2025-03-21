using OpenVision.Core.Configuration;

namespace OpenVision.Core.Features2d.Factory;

internal interface IFeatureExtractorFactory
{
    Feature2D Create(IFeatureExtractorOptions options);
}

internal interface IFeatureMatcherFactory
{
    DescriptorMatcher Create(IFeatureMatcherOptions options);
}
