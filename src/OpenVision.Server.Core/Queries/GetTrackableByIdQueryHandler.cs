using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.Repositories.Specifications;

namespace OpenVision.Server.Core.Queries;

/// <summary>
/// Handles the GetTrackableByIdQuery and returns the target DTO if found.
/// </summary>
public class GetTrackableByIdQueryHandler : IRequestHandler<GetTrackableByIdQuery, TargetRecordDto?>
{
    #region Fields/Consts

    private readonly IImageTargetsRepository _imageTargetsRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTrackableByIdQueryHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTrackableByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="imageTargetsRepository">Repository for accessing image targets.</param>
    /// <param name="currentUserService">The current user service to obtain the current user's identifier.</param>
    /// <param name="mapper">The AutoMapper instance used for mapping entities to DTOs.</param>
    /// <param name="logger">The logger instance used for logging informational messages and errors.</param>
    public GetTrackableByIdQueryHandler(
        IImageTargetsRepository imageTargetsRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<GetTrackableByIdQueryHandler> logger)
    {
        _imageTargetsRepository = imageTargetsRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the GetTrackableByIdQuery request.
    /// Retrieves the image target for the current user based on the target ID,
    /// includes the associated database, and maps the entity to a <see cref="TargetDto"/>.
    /// </summary>
    /// <param name="request">The query request containing the target ID.</param>
    /// <param name="cancellationToken">Token used to cancel the operation if needed.</param>
    /// <returns>
    /// A <see cref="TargetRecordDto"/> representing the target if found; otherwise, null.
    /// </returns>
    public async Task<TargetRecordDto?> Handle(GetTrackableByIdQuery request, CancellationToken cancellationToken)
    {
        var apiKey = _currentUserService.ApiKey;

        _logger.LogInformation("Getting target {TargetId} for user {UserId}", request.TargetId, apiKey);

        var imageTargetForServerApiKeySpecification = new ImageTargetForServerApiKeySpecification(request.TargetId, apiKey)
        {
            Includes =
            {
                target => target.Database,
                target => target.Database.ApiKeys
            }
        };
        var imageTargets = await _imageTargetsRepository.GetBySpecificationAsync(imageTargetForServerApiKeySpecification, cancellationToken);
        var imageTarget = imageTargets.SingleOrDefault();

        if (imageTarget is null)
        {
            _logger.LogWarning("Target {TargetId} not found for user {UserId}", request.TargetId, apiKey);
            return null;
        }

        return _mapper.Map<TargetRecordDto>(imageTarget);
    }

    #endregion
}
