using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.Repositories.Specifications;

namespace OpenVision.Server.Core.Queries;

/// <summary>
/// Handles the <see cref="GetQueryableTrackableQuery"/> request by retrieving image targets
/// filtered by the server API key associated with the current user, and projects them to a queryable collection of <see cref="TargetRecordDto"/> objects.
/// </summary>
public class GetQueryableTrackableQueryHandler : IRequestHandler<GetQueryableTrackableQuery, IQueryable<TargetRecordDto>>
{
    #region Fields/Consts

    private readonly IImageTargetsRepository _imageTargetsRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetQueryableTrackableQueryHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GetQueryableTrackableQueryHandler"/> class.
    /// </summary>
    /// <param name="imageTargetsRepository">The repository for accessing image targets.</param>
    /// <param name="currentUserService">The current user service that provides access to the user's API key.</param>
    /// <param name="mapper">The AutoMapper instance used for projecting entities to DTOs.</param>
    /// <param name="logger">The logger instance for logging informational messages and errors.</param>
    public GetQueryableTrackableQueryHandler(
        IImageTargetsRepository imageTargetsRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<GetQueryableTrackableQueryHandler> logger)
    {
        _imageTargetsRepository = imageTargetsRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the <see cref="GetQueryableTrackableQuery"/> request.
    /// Retrieves image targets that match the server API key provided by the current user,
    /// and projects the results to a queryable collection of <see cref="TargetRecordDto"/> objects.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>
    /// An <see cref="IQueryable{TargetRecordDto}"/> representing the queryable collection of target record DTOs.
    /// </returns>
    public async Task<IQueryable<TargetRecordDto>> Handle(GetQueryableTrackableQuery request, CancellationToken cancellationToken)
    {
        var apiKey = _currentUserService.ApiKey;
        _logger.LogInformation("Getting targets for user with API key: {ApiKey}", apiKey);

        var imageTargetSpec = new ImageTargetForServerApiKeySpecification(apiKey);
        var imageTargetsQueryable = _imageTargetsRepository.GetQueryableBySpecification(imageTargetSpec);
        var targetDtoQueryable = imageTargetsQueryable.ProjectTo<TargetRecordDto>(_mapper.ConfigurationProvider);

        var count = await targetDtoQueryable.CountAsync(cancellationToken);
        _logger.LogInformation("Retrieved {Count} targets for user with API key: {ApiKey}", count, apiKey);

        return targetDtoQueryable;
    }

    #endregion
}