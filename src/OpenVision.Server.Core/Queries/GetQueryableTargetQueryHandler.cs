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
/// Handles the GetTargetsQueryableQuery and returns the list of target DTOs.
/// </summary>
public class GetQueryableTargetQueryHandler : IRequestHandler<GetQueryableTargetQuery, IQueryable<TargetDto>>
{
    #region Fields/Consts

    private readonly IImageTargetsRepository _imageTargetsRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetQueryableTargetQueryHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GetQueryableTargetQueryHandler"/> class.
    /// </summary>
    /// <param name="imageTargetsRepository">The repository for accessing image targets.</param>
    /// <param name="currentUserService">The current user service to obtain the current user's identifier.</param>
    /// <param name="mapper">The AutoMapper instance used for mapping entities to DTOs.</param>
    /// <param name="logger">The logger for logging informational messages and errors.</param>
    public GetQueryableTargetQueryHandler(
        IImageTargetsRepository imageTargetsRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<GetQueryableTargetQueryHandler> logger)
    {
        _imageTargetsRepository = imageTargetsRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the GetTargetsQueryableQuery request.
    /// Retrieves audio assets for the current user, includes associated image assets,
    /// and projects the results to a collection of <see cref="TargetDto"/> objects.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Token used to cancel the operation if needed.</param>
    /// <returns>
    /// An <see cref="IEnumerable{TargetDto}"/> containing the list of target DTOs.
    /// </returns>
    public async Task<IQueryable<TargetDto>> Handle(GetQueryableTargetQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        _logger.LogInformation("Getting targets for user {UserId}", userId);

        var imageTargetForUserSpecification = new ImageTargetForUserSpecification(userId);
        var imageTargetsQueryable = _imageTargetsRepository.GetQueryableBySpecification(imageTargetForUserSpecification);
        var targetDtoQueryable = imageTargetsQueryable.ProjectTo<TargetDto>(_mapper.ConfigurationProvider);

        _logger.LogInformation("Retrieved {Count} targets for user {UserId}", await targetDtoQueryable.CountAsync(cancellationToken), userId);

        return targetDtoQueryable;
    }

    #endregion
}
