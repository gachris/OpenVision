using OpenVision.Core.Dataset;
using OpenVision.Core.Features2d;
using OpenVision.Core.Reco.DataTypes;
using System.Collections.Concurrent;
using System.Data;

namespace OpenVision.Core.Reco;

/// <summary>
/// Class responsible for image recognition using feature extraction and matching.
/// </summary>
public class ImageRecognition : IImageRecognition
{
    #region Fields/Consts

    private readonly Lazy<FeatureExtractor> _featureExtractor;
    private readonly Lazy<FeatureMatcher> _featureMatcher;
    private readonly ImageRequestBuilder _imageRequestBuilder;

    private TargetMatchQuery[]? _targetMatchQueries;
    private bool _isReady;

    #endregion

    #region Properties

    /// <inheritdoc/>
    public bool IsReady => _isReady;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageRecognition"/> class.
    /// </summary>
    public ImageRecognition()
    {
        _featureExtractor = new Lazy<FeatureExtractor>(() => new FeatureExtractor());
        _featureMatcher = new Lazy<FeatureMatcher>(() => new FeatureMatcher());

        _imageRequestBuilder = new ImageRequestBuilder().WithGrayscale()
            .WithGaussianBlur(new System.Drawing.Size(5, 5), 0)
            .WithLowResolution(160);
    }

    /// <inheritdoc/>
    public async Task InitAsync(IEnumerable<ImageData> images)
    {
        _isReady = false;

        _targetMatchQueries = await Task.Run(() =>
            images.AsParallel().Select(GetTargetMatchQuery).ToArray()
        );

        _isReady = true;

        TargetMatchQuery GetTargetMatchQuery(ImageData image)
        {
            var request = _imageRequestBuilder.Build(image.Mat);
            var imageDetectionResult = _featureExtractor.Value.DetectAndCompute(request);

            return new TargetMatchQuery(image.Id,
                                        imageDetectionResult.Mat,
                                        imageDetectionResult.Keypoints,
                                        imageDetectionResult.Descriptors);
        }
    }

    /// <inheritdoc/>
    public Task InitAsync(TargetDataset dataset)
    {
        return InitAsync(dataset.Targets);
    }

    /// <inheritdoc/>
    public async Task InitAsync(IEnumerable<Target> targets)
    {
        _isReady = false;

        _targetMatchQueries = await Task.Run(() =>
            targets.AsParallel().Select(GetTargetMatchQuery).ToArray()
        );

        _isReady = true;

        static TargetMatchQuery GetTargetMatchQuery(Target target)
        {
            return new TargetMatchQuery(
                target.Id,
                target.Image.ToMat(),
                target.Keypoints.KeyPointBytesToMatOfKeyPoint(),
                target.Descriptors.DescriptorBytesToMat(target.DescriptorsRows, target.DescriptorsCols));
        }
    }

    /// <inheritdoc/>
    public FeatureMatchingResult Match(IImageRequest request)
    {
        if (!_isReady)
        {
            throw new InvalidOperationException("Image recognition system is not ready.");
        }

        var frameId = $"frame_{DateTime.Now:ddMMyyyyhhmmss}";
        var targetDetectionResult = _featureExtractor.Value.DetectAndCompute(request);

        var targetMatchQuery = new TargetMatchQuery(
            frameId,
            request.Mat,
            targetDetectionResult.Keypoints,
            targetDetectionResult.Descriptors);

        var targetMatches = new ConcurrentBag<TargetMatchResult>();

        Parallel.ForEach(_targetMatchQueries!, trainInfo =>
        {
            var homographyResult = _featureMatcher.Value.Match(targetMatchQuery, trainInfo);
            if (homographyResult.MatchFound)
            {
                var targetMatch = homographyResult.ToTargetMatchResult(request, targetMatchQuery, trainInfo);
                targetMatches.Add(targetMatch);
            }
        });

        return new FeatureMatchingResult(targetMatches);
    }
}