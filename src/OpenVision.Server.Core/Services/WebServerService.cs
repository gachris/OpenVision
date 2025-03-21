using System.Security.Claims;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenVision.Core.DataTypes;
using OpenVision.Core.Features2d;
using OpenVision.Core.Utils;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Properties;
using OpenVision.Server.Core.Utils;
using OpenVision.Server.EntityFramework.DbContexts;
using OpenVision.Server.EntityFramework.Entities;
using OpenVision.Shared;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Service for interacting with trackable targets with X-API-KEY.
/// </summary>
public class WebServerService : BaseApiService, IWebServerService
{
    #region Fields/Consts

    private readonly ApplicationDbContext _applicationContext;
    private readonly HttpContext _httpContext;
    private readonly IMapper _mapper;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="WebServerService"/> class.
    /// </summary>
    /// <param name="applicationContext">The application database context.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="mapper">The mapper service.</param>
    public WebServerService(ApplicationDbContext applicationContext,
                            IHttpContextAccessor httpContextAccessor,
                            IMapper mapper)
    {
        _applicationContext = applicationContext;
        _httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _mapper = mapper;
    }

    #region IWebServerService Implementation

    /// <inheritdoc/>
    public async Task<GetAllTrackablesResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        var apiKey = _httpContext.User.FindFirstValue(ApiKeyDefaults.X_API_KEY);
        apiKey.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.ApiKeyNotFound);

        var targets = await _applicationContext.ImageTargets
                                               .Where(x => x.Database.ApiKeys.First(key => key.Type == ApiKeyType.Server).Key == apiKey)
                                               .Include(a => a.Database.ApiKeys)
                                               .OrderBy(x => x.Created)
                                               .ToListAsync(cancellationToken);

        var result = targets.Select(_mapper.Map<TargetRecordModel>).ToArray();
        return new GetAllTrackablesResponse(new(result), Guid.NewGuid(), StatusCode.Success, []);
    }

    /// <inheritdoc/>
    public async Task<TrackableRetrieveResponse> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var apiKey = _httpContext.User.FindFirstValue(ApiKeyDefaults.X_API_KEY);
        apiKey.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.ApiKeyNotFound);

        var target = await _applicationContext.ImageTargets
                                              .Include(a => a.Database.ApiKeys)
                                              .SingleOrDefaultAsync(x => x.Id == id && x.Database.ApiKeys.First(key => key.Type == ApiKeyType.Server).Key == apiKey, cancellationToken);

        target.ThrowIfNull(ResultCode.RecordNotFound, ErrorMessages.TargetNotFound);

        var targetResponse = _mapper.Map<TargetRecordModel>(target);
        return new TrackableRetrieveResponse(new ResponseDoc<TargetRecordModel>(targetResponse), Guid.NewGuid(), StatusCode.Success, []);
    }

    /// <inheritdoc/>
    public async Task<PostTrackableResponse> CreateAsync(PostTrackableRequest body, CancellationToken cancellationToken = default)
    {
        var apiKey = _httpContext.User.FindFirstValue(ApiKeyDefaults.X_API_KEY);
        apiKey.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.ApiKeyNotFound);

        var targetId = Guid.NewGuid();

        var database = await _applicationContext.Databases
                                                .Include(a => a.ApiKeys)
                                                .SingleOrDefaultAsync(x => x.ApiKeys.First(key => key.Type == ApiKeyType.Server).Key == apiKey, cancellationToken);

        database.ThrowIfNull(ResultCode.RecordNotFound, ErrorMessages.DatabaseNotFound);

        var imageData = new Regex("^data:image/[a-zA-Z]+;base64,").Replace(body.Image!, string.Empty);
        var buffer = Convert.FromBase64String(imageData);

        var image = buffer.ToMat();

        image.LowResolution(320);

        var requestBuilder = new ImageRequestBuilder().WithGrayscale()
                                                      .WithGaussianBlur(new System.Drawing.Size(5, 5), 0);

        var request = requestBuilder.Build(image);
        var featureExtractor = new FeatureExtractor();
        var imageDetectionInfo = featureExtractor.DetectAndCompute(request);

        var height = UnitsHelper.CalculateYUnits(body.Width!.Value, image.Width, image.Height);

        var target = new ImageTarget
        {
            Id = targetId,
            DatabaseId = database.Id,
            Name = body.Name!,
            Type = TargetType.Cloud,
            Width = body.Width.Value,
            Height = height,
            ActiveFlag = body.ActiveFlag ?? ActiveFlag.True,
            Metadata = body.Metadata,
            PreprocessImage = image.ToArray(),
            AfterProcessImage = imageDetectionInfo.Mat.ToArray(),
            AfterProcessImageWithKeypoints = Features2dHelper.DrawKeypoints(imageDetectionInfo.Mat, imageDetectionInfo.Keypoints, System.Drawing.Color.Red).ToArray(),
            Keypoints = imageDetectionInfo.Keypoints.ToByteArray(),
            Descriptors = imageDetectionInfo.Descriptors.DescriptorToArray(),
            DescriptorsRows = imageDetectionInfo.Descriptors.Rows,
            DescriptorsCols = imageDetectionInfo.Descriptors.Cols,
            Created = DateTimeOffset.Now,
            Updated = DateTimeOffset.Now,
            Database = database
        };

        _applicationContext.ImageTargets.Add(target);
        _applicationContext.SaveChanges();

        return new PostTrackableResponse(new ResponseDoc<Guid>(targetId), Guid.NewGuid(), StatusCode.Success, []);
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage> UpdateAsync(Guid id, PostTrackableRequest body, CancellationToken cancellationToken = default)
    {
        var apiKey = _httpContext.User.FindFirstValue(ApiKeyDefaults.X_API_KEY);
        apiKey.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.ApiKeyNotFound);

        var target = await _applicationContext.ImageTargets.Include(a => a.Database.ApiKeys)
                                                           .SingleOrDefaultAsync(x => x.Id == id && x.Database.ApiKeys.First(key => key.Type == ApiKeyType.Server).Key == apiKey, cancellationToken);

        target.ThrowIfNull(ResultCode.RecordNotFound, ErrorMessages.TargetNotFound);

        target.Name = body.Name ?? target.Name;
        target.Width = body.Width ?? target.Width;

        if (body.Image is not null)
        {
            var imageData = new Regex("^data:image/[a-zA-Z]+;base64,").Replace(body.Image, string.Empty);
            var buffer = Convert.FromBase64String(imageData);

            var image = buffer.ToMat();

            image.LowResolution(320);

            var requestBuilder = new ImageRequestBuilder().WithGrayscale()
                                                          .WithGaussianBlur(new System.Drawing.Size(5, 5), 0);

            var request = requestBuilder.Build(image);
            var featureExtractor = new FeatureExtractor();
            var imageDetectionInfo = featureExtractor.DetectAndCompute(request);

            target.PreprocessImage = image.ToArray();
            target.AfterProcessImage = imageDetectionInfo.Mat.ToArray();
            target.AfterProcessImageWithKeypoints = Features2dHelper.DrawKeypoints(imageDetectionInfo.Mat, imageDetectionInfo.Keypoints, System.Drawing.Color.Red).ToArray();
            target.Keypoints = imageDetectionInfo.Keypoints.ToByteArray();
            target.Descriptors = imageDetectionInfo.Descriptors.DescriptorToArray();
            target.DescriptorsRows = imageDetectionInfo.Descriptors.Rows;
            target.DescriptorsCols = imageDetectionInfo.Descriptors.Cols;
            target.Height = UnitsHelper.CalculateYUnits(target.Width, image.Width, image.Height);
        }

        target.Height = body.Width ?? target.Height;
        target.ActiveFlag = body.ActiveFlag ?? target.ActiveFlag;

        target.Metadata = string.IsNullOrWhiteSpace(body.Metadata) ? null : body.Metadata;

        target.Updated = DateTimeOffset.Now;

        await _applicationContext.SaveChangesAsync(cancellationToken);

        return Success();
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var apiKey = _httpContext.User.FindFirstValue(ApiKeyDefaults.X_API_KEY);
        apiKey.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.ApiKeyNotFound);

        var target = await _applicationContext.ImageTargets.Include(a => a.Database.ApiKeys)
                                                           .SingleOrDefaultAsync(x => x.Id == id && x.Database.ApiKeys.First(key => key.Type == ApiKeyType.Server).Key == apiKey, cancellationToken);

        target.ThrowIfNull(ResultCode.RecordNotFound, ErrorMessages.TargetNotFound);

        _applicationContext.ImageTargets.Remove(target);

        await _applicationContext.SaveChangesAsync(cancellationToken);

        return Success();
    }

    #endregion
}
