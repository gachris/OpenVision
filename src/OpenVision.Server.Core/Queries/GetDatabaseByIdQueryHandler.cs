using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.Repositories.Specifications;

namespace OpenVision.Server.Core.Queries;

/// <summary>
/// Handles the GetDatabaseByIdQuery and returns the database DTO if found.
/// </summary>
public class GetDatabaseByIdQueryHandler : IRequestHandler<GetDatabaseByIdQuery, DatabaseDto?>
{
    #region Fields/Consts

    private readonly IDatabasesRepository _databasesRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDatabaseByIdQueryHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDatabaseByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="databasesRepository">Repository for accessing databases.</param>
    /// <param name="currentUserService">The current user service to obtain the current user's identifier.</param>
    /// <param name="mapper">The AutoMapper instance used for mapping entities to DTOs.</param>
    /// <param name="logger">The logger instance used for logging informational messages and errors.</param>
    public GetDatabaseByIdQueryHandler(
        IDatabasesRepository databasesRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<GetDatabaseByIdQueryHandler> logger)
    {
        _databasesRepository = databasesRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the GetDatabaseByIdQuery request.
    /// Retrieves the database for the current user based on the database ID,
    /// includes associated image asset, and maps the entity to a <see cref="DatabaseDto"/>.
    /// </summary>
    /// <param name="request">The query request containing the database ID.</param>
    /// <param name="cancellationToken">Token used to cancel the operation if needed.</param>
    /// <returns>
    /// A <see cref="DatabaseDto"/> representing the database if found; otherwise, null.
    /// </returns>
    public async Task<DatabaseDto?> Handle(GetDatabaseByIdQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new ArgumentException("User identifier not found.");

        _logger.LogInformation("Getting database {DatabaseId} for user {UserId}", request.DatabaseId, userId);

        var databaseForUserSpecification = new DatabaseForUserSpecification(request.DatabaseId, userId)
        {
            Includes =
            {
                database => database.ImageTargets,
                database => database.ApiKeys
            }
        };
        var databases = await _databasesRepository.GetBySpecificationAsync(databaseForUserSpecification, cancellationToken);
        var database = databases.SingleOrDefault();

        if (database is null)
        {
            _logger.LogWarning("Database {DatabaseId} not found for user {UserId}", request.DatabaseId, userId);
            return null;
        }

        return _mapper.Map<DatabaseDto>(database);
    }

    #endregion
}
