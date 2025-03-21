namespace OpenVision.Core.Features2d.Factory;

internal class SIFTFactory : IFeatureExtractorFactory
{
    public Feature2D Create(IFeatureExtractorOptions options)
    {
        var siftOptions = (SIFTOptions)options;
#if ANDROID
        return SIFT.Create(siftOptions.NumFeatures,
                           siftOptions.NumOctaveLayers,
                           siftOptions.ContrastThreshold,
                           siftOptions.EdgeThreshold,
                           siftOptions.Sigma)!;
#else
        return new SIFT(siftOptions.NumFeatures,
                        siftOptions.NumOctaveLayers,
                        siftOptions.ContrastThreshold,
                        siftOptions.EdgeThreshold,
                        siftOptions.Sigma);
#endif
    }
}
