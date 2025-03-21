using OpenVision.Core.Features2d.Factory;

namespace OpenVision.Core.Features2d;

/// <summary>
/// Feature matcher implementation using OpenCV and EmguCV.
/// </summary>
public class FeatureMatcher : IFeatureMatcher
{
    #region Fields/Consts

    private const int k = 2;
    private const double uniquenessThreshold = 0.8;
    private const double scaleIncrement = 1.5;
    private const int rotationBins = 20;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="FeatureMatcher"/> class.
    /// </summary>
    public FeatureMatcher()
    {
    }

    #region IFeatureMatcher Implementation

    /// <inheritdoc/>
    public HomographyResult Match(TargetMatchQuery queryInfo, TargetMatchQuery trainInfo)
    {
        using var featuresDescriptor = CreateDescriptorMatcher();
        var homography = default(Mat);
#if ANDROID
        var matches = new MatOfDMatch();
        featuresDescriptor.Match(queryInfo.Descriptors, trainInfo.Descriptors, matches);

        var listOfGoodMatches = matches.ToArray()!;

        var mask = new Mat(listOfGoodMatches.Length, 1, CvType.Cv8u);
        mask.SetTo(new Scalar(255));

        //OpenCVHelper.VoteForUniqueness(matches, uniquenessThreshold, mask);

        var nonZeroCount = OpenCV.Core.Core.CountNonZero(mask);
        if (nonZeroCount < 4) return new HomographyResult(homography);

        nonZeroCount = OpenCVHelper.VoteForSizeAndOrientation(trainInfo.Keypoints, queryInfo.Keypoints, matches, mask, scaleIncrement, rotationBins);
        if (nonZeroCount < 36) return new HomographyResult(homography);

        homography = OpenCVHelper.GetHomography(queryInfo, trainInfo, listOfGoodMatches, mask);
#else
        var matches = new VectorOfVectorOfDMatch();
        featuresDescriptor.KnnMatch(queryInfo.Descriptors, trainInfo.Descriptors, matches, k);

        var mask = new Mat(matches.Size, 1, DepthType.Cv8U, 1);
        mask.SetTo(new MCvScalar(255));

        Features2DToolbox.VoteForUniqueness(matches, uniquenessThreshold, mask);

        var nonZeroCount = CvInvoke.CountNonZero(mask);
        if (nonZeroCount < 4) return new HomographyResult(homography);

        nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(trainInfo.Keypoints, queryInfo.Keypoints, matches, mask, scaleIncrement, rotationBins);
        if (nonZeroCount < 36) return new HomographyResult(homography);

        homography = OpenCVHelper.GetHomography(queryInfo, trainInfo, matches, mask);
#endif
        return new HomographyResult(homography);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates a descriptor matcher based on the configured options.
    /// </summary>
    /// <returns>The descriptor matcher instance.</returns>
    private static DescriptorMatcher CreateDescriptorMatcher()
    {
        var featureMatcherFactory = FeatureMatcherFactory.Create(VisionSystemConfig.FeatureMatcherOptions.Type);
        return featureMatcherFactory.Create(VisionSystemConfig.FeatureMatcherOptions);
    }

    #endregion
}