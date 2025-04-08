using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenVision.Core.Configuration;
using OpenVision.Core.Features2d;
using OpenVision.Core.Utils;
using OpenVision.EntityFramework.DbContexts;
using OpenVision.EntityFramework.Entities;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Exceptions;
using OpenVision.Server.Core.Helpers;
using OpenVision.Shared;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;
using Error = OpenVision.Shared.Responses.Error;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Service for interacting with trackable targets with X-API-KEY.
/// </summary>
public class WebServerService : IWebServerService
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

    /// <summary>
    /// Initializes a new instance of the <see cref="WebServerService"/> class.
    /// </summary>
    /// <param name="applicationContext">The application database context.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="mapper">The mapper service.</param>
    public WebServerService(IDbContextFactory<ApplicationDbContext> applicationContextPool,
                            IHttpContextAccessor httpContextAccessor,
                            IMapper mapper)
    {
        _applicationContext = applicationContextPool.CreateDbContext();
        _httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _mapper = mapper;
    }

    #region IWebServerService Implementation

    /// <inheritdoc/>
    public async Task<GetAllTrackablesResponse> GetAsync(CancellationToken cancellationToken = default)
    {
        var apiKey = _httpContext.User.FindFirstValue(ApiKeyDefaults.X_API_KEY);
        ThrowIfNullOrEmpty(apiKey, ResultCode.InvalidRequest, "Api key not found.");

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
        ThrowIfNullOrEmpty(apiKey, ResultCode.InvalidRequest, "Api key not found.");

        var target = await _applicationContext.ImageTargets
                                              .Include(a => a.Database.ApiKeys)
                                              .SingleOrDefaultAsync(x => x.Id == id && x.Database.ApiKeys.First(key => key.Type == ApiKeyType.Server).Key == apiKey, cancellationToken);

        ThrowIfNull(target, ResultCode.RecordNotFound, "Target not found.");

        var targetResponse = _mapper.Map<TargetRecordModel>(target);
        return new TrackableRetrieveResponse(new ResponseDoc<TargetRecordModel>(targetResponse), Guid.NewGuid(), StatusCode.Success, []);
    }

    /// <inheritdoc/>
    public async Task<PostTrackableResponse> CreateAsync(PostTrackableRequest body, CancellationToken cancellationToken = default)
    {
        var apiKey = _httpContext.User.FindFirstValue(ApiKeyDefaults.X_API_KEY);
        ThrowIfNullOrEmpty(apiKey, ResultCode.InvalidRequest, "Api key not found.");

        var targetId = Guid.NewGuid();

        var database = await _applicationContext.Databases
                                                .Include(a => a.ApiKeys)
                                                .SingleOrDefaultAsync(x => x.ApiKeys.First(key => key.Type == ApiKeyType.Server).Key == apiKey, cancellationToken);

        ThrowIfNull(database, ResultCode.RecordNotFound, "Database not found.");

        var imageData = new Regex("^data:image/[a-zA-Z]+;base64,").Replace(body.Image!, string.Empty);
        var buffer = Convert.FromBase64String(imageData);

        var image = buffer.ToMat();

        image.LowResolution(320);

        var request = VisionSystemConfig.ImageRequestBuilder.Build(image);
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
            Database = database,
            Rating = 5,
            Recos = 0
        };

        _applicationContext.ImageTargets.Add(target);
        _applicationContext.SaveChanges();

        return new PostTrackableResponse(new ResponseDoc<Guid>(targetId), Guid.NewGuid(), StatusCode.Success, []);
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage> UpdateAsync(Guid id, UpdateTrackableRequest body, CancellationToken cancellationToken = default)
    {
        var apiKey = _httpContext.User.FindFirstValue(ApiKeyDefaults.X_API_KEY);
        ThrowIfNullOrEmpty(apiKey, ResultCode.InvalidRequest, "Api key not found.");

        var target = await _applicationContext.ImageTargets.Include(a => a.Database.ApiKeys)
                                                           .SingleOrDefaultAsync(x => x.Id == id && x.Database.ApiKeys.First(key => key.Type == ApiKeyType.Server).Key == apiKey, cancellationToken);

        ThrowIfNull(target, ResultCode.RecordNotFound, "Target not found.");

        target.Name = body.Name ?? target.Name;
        target.Width = body.Width ?? target.Width;

        if (body.Image is not null)
        {
            var imageData = new Regex("^data:image/[a-zA-Z]+;base64,").Replace(body.Image, string.Empty);
            var buffer = Convert.FromBase64String(imageData);

            var image = buffer.ToMat();

            image.LowResolution(320);

            var request = VisionSystemConfig.ImageRequestBuilder.Build(image);
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
        ThrowIfNullOrEmpty(apiKey, ResultCode.InvalidRequest, "Api key not found.");

        var target = await _applicationContext.ImageTargets.Include(a => a.Database.ApiKeys)
                                                           .SingleOrDefaultAsync(x => x.Id == id && x.Database.ApiKeys.First(key => key.Type == ApiKeyType.Server).Key == apiKey, cancellationToken);

        ThrowIfNull(target, ResultCode.RecordNotFound, "Target not found.");

        _applicationContext.ImageTargets.Remove(target);

        await _applicationContext.SaveChangesAsync(cancellationToken);

        return Success();
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Creates a success response message with no result.
    /// </summary>
    /// <returns>A success response message with no result.</returns>
    protected static IResponseMessage Success()
    {
        return new ResponseMessage(Guid.NewGuid(), StatusCode.Success, []);
    }

    /// <summary>
    /// Creates a success response message with a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The result.</param>
    /// <returns>A success response message with the specified result.</returns>
    protected static IResponseMessage<TResult> Success<TResult>(TResult result)
    {
        return new ResponseMessage<TResult>(new(result), Guid.NewGuid(), StatusCode.Success, []);
    }

    /// <summary>
    /// Throws an exception with the specified result code and message if the specified value is null.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="resultCode">The result code for the exception.</param>
    /// <param name="message">The message for the exception.</param>
    public static void ThrowIfNull<T>([NotNull] T? value, ResultCode resultCode, string message)
    {
        if (value is null)
        {
            throw ThrowHttpException(resultCode, message);
        }
    }

    /// <summary>
    /// Throws an exception with the specified result code and message if the specified string is null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="resultCode">The result code for the exception.</param>
    /// <param name="message">The message for the exception.</param>
    public static void ThrowIfNullOrEmpty([NotNull] string? value, ResultCode resultCode, string message)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw ThrowHttpException(resultCode, message);
        }
    }

    /// <summary>
    /// Throws an <see cref="HttpException"/> with the specified <paramref name="resultCode"/> and <paramref name="message"/>.
    /// </summary>
    /// <param name="resultCode">The result code for the exception.</param>
    /// <param name="message">The message for the exception.</param>
    /// <returns>The <see cref="HttpException"/> with the specified <paramref name="resultCode"/> and <paramref name="message"/>.</returns>
    private static HttpException ThrowHttpException(ResultCode resultCode, string message)
    {
        var errorCollection = new List<Error>();

        var error = new Error(resultCode, message);

        errorCollection.Add(error);

        var errorResponseMessage = new ResponseMessage(Guid.NewGuid(), StatusCode.Failed, errorCollection);

        return new HttpException(errorResponseMessage);
    }

    #endregion
}
