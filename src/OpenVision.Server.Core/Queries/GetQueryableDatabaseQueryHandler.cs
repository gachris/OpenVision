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
/// Handles the <see cref="GetQueryableDatabaseQuery"/> request by retrieving 
/// database entities for the current user and projecting them into a queryable collection of <see cref="DatabaseDto"/> objects.
/// </summary>
public class GetQueryableDatabaseQueryHandler : IRequestHandler<GetQueryableDatabaseQuery, IQueryable<DatabaseDto>>
{
    #region Fields/Consts

    private readonly IDatabasesRepository _databasesRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetQueryableDatabaseQueryHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GetQueryableDatabaseQueryHandler"/> class.
    /// </summary>
    /// <param name="databasesRepository">The repository for accessing database entities.</param>
    /// <param name="currentUserService">The current user service that provides the current user's identifier.</param>
    /// <param name="mapper">The AutoMapper instance used for mapping entities to DTOs.</param>
    /// <param name="logger">The logger for logging informational messages and errors.</param>
    public GetQueryableDatabaseQueryHandler(
        IDatabasesRepository databasesRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<GetQueryableDatabaseQueryHandler> logger)
    {
        _databasesRepository = databasesRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the <see cref="GetQueryableDatabaseQuery"/> request.
    /// Retrieves database entities for the current user by applying the appropriate specification, 
    /// and projects the resulting query to a collection of <see cref="DatabaseDto"/> objects.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation if needed.</param>
    /// <returns>
    /// An <see cref="IQueryable{DatabaseDto}"/> representing the queryable collection of database DTOs.
    /// </returns>
    public async Task<IQueryable<DatabaseDto>> Handle(GetQueryableDatabaseQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        _logger.LogInformation("Retrieving databases for user with ID {UserId}", userId);

        var databaseForUserSpecification = new DatabaseForUserSpecification(userId);
        var databaseQueryable = _databasesRepository.GetQueryableBySpecification(databaseForUserSpecification);
        var databaseDtoQueryable = databaseQueryable.ProjectTo<DatabaseDto>(_mapper.ConfigurationProvider);

        var count = await databaseDtoQueryable.CountAsync(cancellationToken);
        _logger.LogInformation("Retrieved {Count} databases for user with ID {UserId}", count, userId);

        return databaseDtoQueryable;
    }

    #endregion
}
