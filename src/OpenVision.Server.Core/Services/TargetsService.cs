using System.Data;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpenVision.Core.Configuration;
using OpenVision.Core.Features2d;
using OpenVision.Core.Utils;
using OpenVision.Server.Core.Properties;
using OpenVision.Server.Core.Utils;
using OpenVision.Server.EntityFramework.DbContexts;
using OpenVision.Server.EntityFramework.Entities;
using OpenVision.Shared;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;
using OpenVision.Web.Core.Filters;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Service for handling operations related to targets.
/// </summary>
public class TargetsService : BaseApiService, ITargetsService
{
    #region Fields/Consts

    private readonly ApplicationDbContext _applicationContext;
    private readonly HttpContext _httpContext;
    private readonly IUriService _uriService;
    private readonly IMapper _mapper;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetsService"/> class.
    /// </summary>
    /// <param name="applicationContext">The application database context.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="uriService">The URI service.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public TargetsService(ApplicationDbContext applicationContext,
                          IHttpContextAccessor httpContextAccessor,
                          IUriService uriService,
                          IMapper mapper)
    {
        _applicationContext = applicationContext;
        _httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _uriService = uriService;
        _mapper = mapper;
    }

    #region ITargetsService Implementation

    /// <inheritdoc/>
    public async Task<TargetPagedResponse> GetAsync(TargetBrowserQuery query, CancellationToken cancellationToken = default)
    {
        var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        userId.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.UserNotFound);

        var route = _httpContext.Request.Path.Value;
        route.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.RouteNotFound);

        var validFilter = new PaginationFilter(query.Page, query.Size);
        var take = validFilter.Size;
        var skip = validFilter.Page - 1;

        var targets = await _applicationContext.ImageTargets.Where(x => x.Database.UserId == userId && x.Database.Id == query.DatabaseId)
                                                            .Include(a => a.Database)
                                                            .OrderBy(x => x.Created)
                                                            .Skip(skip * take)
                                                            .Take(take)
                                                            .ToListAsync(cancellationToken);

        var totalRecords = await _applicationContext.ImageTargets.CountAsync(cancellationToken);
        var result = targets.Select(_mapper.Map<TargetResponse>);

        var totalPages = totalRecords / (double)validFilter.Size;
        var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));
        var nextPage =
            validFilter.Page >= 1 && validFilter.Page < roundedTotalPages
            ? _uriService.GetPageUri(new PaginationFilter(validFilter.Page + 1, validFilter.Size), route)
            : null;

        var previousPage =
            validFilter.Page - 1 >= 1 && validFilter.Page <= roundedTotalPages
            ? _uriService.GetPageUri(new PaginationFilter(validFilter.Page - 1, validFilter.Size), route)
            : null;

        var firstPage = _uriService.GetPageUri(new PaginationFilter(1, validFilter.Size), route);
        var lastPage = _uriService.GetPageUri(new PaginationFilter(roundedTotalPages, validFilter.Size), route);

        var respose = new TargetPagedResponse(totalPages == 0 ? -1 : validFilter.Page,
                                              totalRecords == 0 ? -1 : validFilter.Size,
                                              totalPages == 0 ? null : firstPage,
                                              totalRecords == 0 ? null : lastPage,
                                              roundedTotalPages,
                                              totalRecords,
                                              nextPage,
                                              previousPage,
                                              new ResponseDoc<IEnumerable<TargetResponse>>(result),
                                              Guid.NewGuid(),
                                              StatusCode.Success,
                                              []);

        return respose;
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<TargetResponse>> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        userId.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.UserNotFound);

        var target = await _applicationContext.ImageTargets
                                              .Include(x => x.Database)
                                              .SingleOrDefaultAsync(x => x.Id == id && x.Database.UserId == userId, cancellationToken);

        target.ThrowIfNull(ResultCode.RecordNotFound, ErrorMessages.TargetNotFound);

        var targetResponse = _mapper.Map<TargetResponse>(target);
        return Success(targetResponse);
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<Guid>> CreateAsync(PostTargetRequest body, CancellationToken cancellationToken = default)
    {
        var targetId = Guid.NewGuid();

        var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        userId.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.UserNotFound);

        var database = await _applicationContext.Databases.FirstOrDefaultAsync(x => x.Id == body.DatabaseId, cancellationToken: cancellationToken);
        database.ThrowIfNull(ResultCode.InvalidRequest, ErrorMessages.DatabaseNotFound);

        var image = body.Image!.ToMat();

        image.LowResolution(320);

        var request = VisionSystemConfig.ImageRequestBuilder.Build(image);
        var featureExtractor = new FeatureExtractor();
        var imageDetectionInfo = featureExtractor.DetectAndCompute(request);

        var height = UnitsHelper.CalculateYUnits(body.Width!.Value, image.Width, image.Height);

        var target = new ImageTarget
        {
            Id = targetId,
            DatabaseId = body.DatabaseId!.Value,
            Name = body.Name!,
            Type = database.Type is DatabaseType.Cloud ? TargetType.Cloud : body.Type!.Value,
            Width = body.Width.Value,
            Height = height,
            Metadata = body.Metadata,
            ActiveFlag = body.ActiveFlag ?? ActiveFlag.True,
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

        return Success(targetId);
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage> UpdateAsync(Guid id, UpdateTargetRequest body, CancellationToken cancellationToken = default)
    {
        var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        userId.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.UserNotFound);

        var target = await _applicationContext.ImageTargets.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        target.ThrowIfNull(ResultCode.RecordNotFound, ErrorMessages.TargetNotFound);

        target.Name = body.Name ?? target.Name;
        target.Width = body.Width ?? target.Width;

        if (body.Image is not null)
        {
            var image = body.Image.ToMat();

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
        else if (body.Width.HasValue)
        {
            var image = target.PreprocessImage.ToMat();
            target.Height = UnitsHelper.CalculateYUnits(body.Width.Value, image.Width, image.Height);
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
        var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        userId.ThrowIfNullOrEmpty(ResultCode.InvalidRequest, ErrorMessages.UserNotFound);

        var target = await _applicationContext.ImageTargets.Include(x => x.Database)
                                                           .SingleOrDefaultAsync(x => x.Id == id && x.Database.UserId == userId, cancellationToken);

        target.ThrowIfNull(ResultCode.RecordNotFound, ErrorMessages.TargetNotFound);

        _applicationContext.ImageTargets.Remove(target);

        await _applicationContext.SaveChangesAsync(cancellationToken);

        return Success();
    }

    #endregion
}
