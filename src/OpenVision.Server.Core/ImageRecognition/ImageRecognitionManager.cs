using OpenVision.Core.Dataset;
using OpenVision.Core.Reco.DataTypes.Requests;
using OpenVision.Core.Reco.DataTypes.Responses;
using OpenVision.Core.Utils;
using OpenVision.Server.Core.Contracts;
using OpenVision.Shared.Responses;
using OpenVision.Shared.Types;

namespace OpenVision.Server.Core.ImageRecognition;

/// <summary>
/// Manages image recognition initialization and feature matching operations.
/// </summary>
public class ImageRecognitionManager : IImageRecognitionManager
{
    #region Fields/Consts

    private readonly OpenVision.Core.Reco.ImageRecognition _imageRecognition = new();
    private readonly IDatabasesRepository _databasesRepository;
    private readonly ICurrentUserService _currentUserService;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageRecognitionManager"/> class.
    /// </summary>
    /// <param name="databasesRepository">The repository for accessing databases.</param>
    /// <param name="currentUserService">The service providing current user information.</param>
    public ImageRecognitionManager(IDatabasesRepository databasesRepository, ICurrentUserService currentUserService)
    {
        _databasesRepository = databasesRepository;
        _currentUserService = currentUserService;
    }

    #region Methods

    /// <summary>
    /// Initializes the image recognition system by retrieving the active database for the current client's API key 
    /// and loading active image targets for feature matching.
    /// </summary>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    public async Task InitializeAsync()
    {
        var clientApiKey = _currentUserService.ApiKey;
        var databasesQueryable = await _databasesRepository.GetAsync();
        var database = databasesQueryable.FirstOrDefault(database =>
            database.ApiKeys.Any(apiKey => apiKey.Key == clientApiKey && apiKey.Type == ApiKeyType.Client));

        ArgumentNullException.ThrowIfNull(database, nameof(database));

        var targets = database.ImageTargets
            .Where(x => x.DatabaseId == database.Id && x.ActiveFlag == ActiveFlag.True)
            .Select(imageTarget => new Target(
              imageTarget.Id.ToString(),
              imageTarget.AfterProcessImage,
              imageTarget.Keypoints,
              imageTarget.Descriptors,
              imageTarget.DescriptorsRows,
              imageTarget.DescriptorsCols,
              imageTarget.Width,
              imageTarget.Height));

        await _imageRecognition.InitAsync(targets);
    }

    /// <summary>
    /// Processes a feature matching request by converting the provided WebSocket match request into a cloud image 
    /// request, performing the match using the underlying image recognition engine, and formatting the response.
    /// </summary>
    /// <param name="matchRequest">The feature matching request containing image data and matching parameters.</param>
    /// <returns>
    /// A <see cref="FeatureMatchingResponse"/> that includes a collection of target matches, a unique identifier for the transaction, 
    /// a status code, and a list of errors (if any).
    /// </returns>
    public FeatureMatchingResponse MatchFeatures(WSMatchRequest matchRequest)
    {
        var cloudImageRequest = new CloudImageRequest
        {
            Id = matchRequest.Id,
            Mat = matchRequest.Mat,
            OriginalWidth = matchRequest.OriginalWidth,
            OriginalHeight = matchRequest.OriginalHeight,
            IsGrayscale = matchRequest.IsGrayscale,
            IsLowResolution = matchRequest.IsLowResolution,
            HasRoi = matchRequest.HasRoi,
            HasGaussianBlur = matchRequest.HasGaussianBlur
        };

        var featureMatchingResult = _imageRecognition.Match(cloudImageRequest);

        var matches = featureMatchingResult.Matches.Select(match =>
            new TargetMatchResponse(
                match.Id,
                match.ProjectedRegion,
                match.Size,
                match.CenterX,
                match.CenterY,
                match.Angle,
                match.HomographyArray))
            .ToArray();

        var featureMatchingResponse = new FeatureMatchingResponse(
            new ResponseDoc<IReadOnlyCollection<TargetMatchResponse>>(matches),
            Guid.NewGuid(),
            StatusCode.Success,
            []); // Assuming an empty array represents no errors

        return featureMatchingResponse;
    }

    #endregion
}