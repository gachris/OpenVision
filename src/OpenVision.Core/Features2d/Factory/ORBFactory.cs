namespace OpenVision.Core.Features2d.Factory;

internal class ORBFactory : IFeatureExtractorFactory
{
    public Feature2D Create(IFeatureExtractorOptions options)
    {
        var orbOptions = (ORBOptions)options;
#if ANDROID
        return ORB.Create(orbOptions.NumberOfFeatures,
                          orbOptions.ScaleFactor,
                          orbOptions.NumLevels,
                          orbOptions.EdgeThreshold,
                          orbOptions.FirstLevel,
                          orbOptions.WTK_A,
                          (int)orbOptions.Score,
                          orbOptions.PatchSize,
                          orbOptions.FastThreshold)!;
#else
        return new ORB(orbOptions.NumberOfFeatures,
                       orbOptions.ScaleFactor,
                       orbOptions.NumLevels,
                       orbOptions.EdgeThreshold,
                       orbOptions.FirstLevel,
                       orbOptions.WTK_A,
                       (ORB.ScoreType)orbOptions.Score,
                       orbOptions.PatchSize,
                       orbOptions.FastThreshold);
#endif
    }
}
