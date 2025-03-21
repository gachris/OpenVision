using OpenVision.Core.Configuration;

namespace OpenVision.Core.Features2d.Factory;

internal class KAZEFactory : IFeatureExtractorFactory
{
    public Feature2D Create(IFeatureExtractorOptions options)
    {
        var kazeOptions = (KAZEOptions)options;
#if ANDROID
        return KAZE.Create(kazeOptions.Extended,
                           kazeOptions.Upright,
                           kazeOptions.Threshold,
                           kazeOptions.Octaves,
                           kazeOptions.Sublevels,
                           (int)kazeOptions.Diffusivity)!;
#else
        return new KAZE(kazeOptions.Extended,
                        kazeOptions.Upright,
                        kazeOptions.Threshold,
                        kazeOptions.Octaves,
                        kazeOptions.Sublevels,
                        (KAZE.Diffusivity)kazeOptions.Diffusivity);
#endif
    }
}
