using OpenVision.Core.Configuration;

namespace OpenVision.Core.Features2d.Factory;

internal class BRISKFactory : IFeatureExtractorFactory
{
    public Feature2D Create(IFeatureExtractorOptions options)
    {
        var briskOptions = (BRISKOptions)options;
#if ANDROID
        return BRISK.Create(briskOptions.Thresh, briskOptions.Octaves, briskOptions.PatternScale)!;
#else
        return new Brisk(briskOptions.Thresh, briskOptions.Octaves, briskOptions.PatternScale);
#endif
    }
}
