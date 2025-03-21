namespace OpenVision.Core.Features2d.Factory;

internal class GFTTDetectorFactory : IFeatureExtractorFactory
{
    public Feature2D Create(IFeatureExtractorOptions options)
    {
        var gfttOptions = (GFTTDetectorOptions)options;
#if ANDROID
        return GFTTDetector.Create(gfttOptions.MaxCorners,
                                   gfttOptions.QualityLevel,
                                   gfttOptions.MinDistance,
                                   gfttOptions.BlockSize,
                                   gfttOptions.UseHarrisDetector,
                                   gfttOptions.K)!;
#else
        return new GFTTDetector(gfttOptions.MaxCorners,
                                gfttOptions.QualityLevel,
                                gfttOptions.MinDistance,
                                gfttOptions.BlockSize,
                                gfttOptions.UseHarrisDetector,
                                gfttOptions.K);
#endif
    }
}
