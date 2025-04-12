using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Queries;

/// <summary>
/// Handles the GetTargetsQuery and returns the list of target DTOs.
/// </summary>
public class GetTargetsQueryHandler : IRequestHandler<GetTargetsQuery, IEnumerable<TargetDto>>
{
    #region Fields/Consts

    private readonly IImageTargetsRepository _imageTargetsRepository;
    private readonly ITargetSpecificationFactory _targetSpecificationFactory;
    private readonly IMapper _mapper;
    private readonly ILogger<GetTargetsQueryHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTargetsQueryHandler"/> class.
    /// </summary>
    /// <param name="imageTargetsRepository">The repository for accessing image targets.</param>
    /// <param name="targetSpecificationFactory">The factory used to create the appropriate target specification based on the current authentication context.</param>
    /// <param name="mapper">The AutoMapper instance used for mapping entities to DTOs.</param>
    /// <param name="logger">The logger for logging informational messages and errors.</param>
    public GetTargetsQueryHandler(
        IImageTargetsRepository imageTargetsRepository,
        ITargetSpecificationFactory targetSpecificationFactory,
        IMapper mapper,
        ILogger<GetTargetsQueryHandler> logger)
    {
        _imageTargetsRepository = imageTargetsRepository;
        _targetSpecificationFactory = targetSpecificationFactory;
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the GetTargetsQuery request.
    /// Retrieves targets for the current user, includes the associated database,
    /// and projects the results to a collection of <see cref="TargetDto"/> objects.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>
    /// An <see cref="IEnumerable{TargetDto}"/> containing the list of target DTOs.
    /// </returns>
    public async Task<IEnumerable<TargetDto>> Handle(GetTargetsQuery request, CancellationToken cancellationToken)
    {
        var imageTargetForUserSpecification = _targetSpecificationFactory.GetImageTargetSpecification();
        var imageTargets = await _imageTargetsRepository.GetBySpecificationAsync(imageTargetForUserSpecification, cancellationToken);
        var targetDtos = _mapper.Map<IEnumerable<TargetDto>>(imageTargets);

        _logger.LogInformation("Retrieved {Count} targets", targetDtos.Count());

        return targetDtos;
    }

    #endregion
}