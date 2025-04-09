using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.Repositories.Specifications;

namespace OpenVision.Server.Core.Queries;

/// <summary>
/// Handles the GetTrackablesQuery and returns the list of target DTOs.
/// </summary>
public class GetTrackablesQueryHandler : IRequestHandler<GetTrackablesQuery, IEnumerable<TargetRecordDto>>
{
    #region Fields/Consts

    private readonly IImageTargetsRepository _imageTargetsRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTrackablesQueryHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTrackablesQueryHandler"/> class.
    /// </summary>
    /// <param name="imageTargetsRepository">The repository for accessing image targets.</param>
    /// <param name="currentUserService">The current user service to obtain the current user's identifier.</param>
    /// <param name="mapper">The AutoMapper instance used for mapping entities to DTOs.</param>
    /// <param name="logger">The logger for logging informational messages and errors.</param>
    public GetTrackablesQueryHandler(
        IImageTargetsRepository imageTargetsRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<GetTrackablesQueryHandler> logger)
    {
        _imageTargetsRepository = imageTargetsRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the GetTrackablesQuery request.
    /// Retrieves targets for the current user, includes the associated database,
    /// and projects the results to a collection of <see cref="TargetRecordDto"/> objects.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>
    /// An <see cref="IEnumerable{TargetRecordDto}"/> containing the list of target DTOs.
    /// </returns>
    public async Task<IEnumerable<TargetRecordDto>> Handle(GetTrackablesQuery request, CancellationToken cancellationToken)
    {
        var apiKey = _currentUserService.ApiKey;

        _logger.LogInformation("Getting targets for user {UserId}", apiKey);

        var imageTargetForServerApiKeySpecification = new ImageTargetForServerApiKeySpecification(apiKey)
        {
            Includes =
            {
                target => target.Database,
                target => target.Database.ApiKeys
            }
        };
        var imageTargets = await _imageTargetsRepository.GetBySpecificationAsync(imageTargetForServerApiKeySpecification, cancellationToken);
        var targetRecordDtos = _mapper.Map<IEnumerable<TargetRecordDto>>(imageTargets);

        _logger.LogInformation("Retrieved {Count} targets for user {UserId}", imageTargets.Count(), imageTargets);

        return targetRecordDtos;
    }

    #endregion
}
