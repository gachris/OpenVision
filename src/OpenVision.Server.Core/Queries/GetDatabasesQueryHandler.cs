﻿using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.Repositories.Specifications;

namespace OpenVision.Server.Core.Queries;

/// <summary>
/// Handles the GetDatabasesQuery and returns the list of database DTOs.
/// </summary>
public class GetDatabasesQueryHandler : IRequestHandler<GetDatabasesQuery, IEnumerable<DatabaseDto>>
{
    #region Fields/Consts

    private readonly IDatabasesRepository _databasesRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDatabasesQueryHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDatabasesQueryHandler"/> class.
    /// </summary>
    /// <param name="databasesRepository">The repository for accessing databases.</param>
    /// <param name="currentUserService">The current user service to obtain the current user's identifier.</param>
    /// <param name="mapper">The AutoMapper instance used for mapping entities to DTOs.</param>
    /// <param name="logger">The logger for logging informational messages and errors.</param>
    public GetDatabasesQueryHandler(
        IDatabasesRepository databasesRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<GetDatabasesQueryHandler> logger)
    {
        _databasesRepository = databasesRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the GetDatabasesQuery request.
    /// Retrieves databases for the current user, includes associated api keys and image targets,
    /// and projects the results to a collection of <see cref="DatabaseDto"/> objects.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">Token used to cancel the operation if needed.</param>
    /// <returns>
    /// An <see cref="IEnumerable{DatabaseDto}"/> containing the list of database DTOs.
    /// </returns>
    public async Task<IEnumerable<DatabaseDto>> Handle(GetDatabasesQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new ArgumentException("User identifier not found.");

        _logger.LogInformation("Getting databases for user {UserId}", userId);

        var databaseForUserSpecification = new DatabaseForUserSpecification(userId)
        {
            Includes =
            {
                database => database.ImageTargets,
                database => database.ApiKeys
            }
        };
        var databases = await _databasesRepository.GetBySpecificationAsync(databaseForUserSpecification, cancellationToken);
        var databaseDtos = _mapper.Map<IEnumerable<DatabaseDto>>(databases);

        _logger.LogInformation("Retrieved {Count} databases for user {UserId}", databases.Count(), userId);

        return databaseDtos;
    }

    #endregion
}
