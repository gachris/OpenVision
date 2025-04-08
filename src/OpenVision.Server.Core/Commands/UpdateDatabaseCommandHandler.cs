using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Handles the UpdateDatabaseCommand and updates the database.
/// </summary>
public class UpdateDatabaseCommandHandler : IRequestHandler<UpdateDatabaseCommand, DatabaseDto>
{
    #region Fields/Consts

    private readonly IDatabasesRepository _databasesRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateDatabaseCommandHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDatabaseCommandHandler"/> class.
    /// </summary>
    /// <param name="databasesRepository">The repository for accessing databases.</param>
    /// <param name="currentUserService">The service for obtaining the current user's identifier.</param>
    /// <param name="mapper">The AutoMapper instance for mapping between entities and DTOs.</param>
    /// <param name="logger">The logger for recording informational and error messages.</param>
    public UpdateDatabaseCommandHandler(
        IDatabasesRepository databasesRepository,
        ICurrentUserService currentUserService,
        IMapper mapper,
        ILogger<UpdateDatabaseCommandHandler> logger)
    {
        _databasesRepository = databasesRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the UpdateDatabaseCommand request by updating the database details.
    /// </summary>
    /// <param name="request">The command containing the database ID and update details.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A <see cref="DatabaseDto"/> representing the updated database.</returns>
    public async Task<DatabaseDto> Handle(UpdateDatabaseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        _logger.LogInformation("Editing database {DatabaseId} for user {UserId}", request.DatabaseId, userId);

        var databasesQueryable = await _databasesRepository.GetAsync();
        var database = await databasesQueryable
            .Where(x => x.Id == request.DatabaseId && x.UserId == userId)
            .SingleOrDefaultAsync(cancellationToken);

        if (database is null)
        {
            _logger.LogWarning("Database {DatabaseId} not found for user {UserId}", request.DatabaseId, userId);
            throw new ArgumentNullException(nameof(database));
        }

        database.Name = request.UpdateDatabaseDto.Name ?? database.Name;
        database.Updated = DateTimeOffset.Now;

        await _databasesRepository.UpdateAsync(database, cancellationToken);
        _logger.LogInformation("Edited database {DatabaseId} for user {UserId}", request.DatabaseId, userId);

        return _mapper.Map<DatabaseDto>(database);
    }

    #endregion
}
