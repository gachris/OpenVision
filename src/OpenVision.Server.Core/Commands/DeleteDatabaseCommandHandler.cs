using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Repositories.Specifications;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Handles the DeleteDatabaseCommand and deletes the database.
/// </summary>
public class DeleteDatabaseCommandHandler : IRequestHandler<DeleteDatabaseCommand, bool>
{
    #region Fields/Consts

    private readonly IDatabasesRepository _databasesRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteDatabaseCommandHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteDatabaseCommandHandler"/> class.
    /// </summary>
    /// <param name="databasesRepository">The repository for accessing databases.</param>
    /// <param name="logger">The logger for recording informational and error messages.</param>
    /// <param name="currentUserService">The service for obtaining the current user's identifier.</param>
    public DeleteDatabaseCommandHandler(
        IDatabasesRepository databasesRepository,
        ILogger<DeleteDatabaseCommandHandler> logger,
        ICurrentUserService currentUserService)
    {
        _databasesRepository = databasesRepository;
        _logger = logger;
        _currentUserService = currentUserService;
    }

    #region Methods

    /// <summary>
    /// Handles the DeleteDatabaseCommand request by deleting the database.
    /// </summary>
    /// <param name="request">The command containing the database ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A boolean value indicating whether the deletion was successful.</returns>
    public async Task<bool> Handle(DeleteDatabaseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        _logger.LogInformation("Deleting database {DatabaseId} for user {UserId}", request.DatabaseId, userId);

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
            return false;
        }

        var result = await _databasesRepository.RemoveAsync(database, cancellationToken);
        _logger.LogInformation("Deleted database {DatabaseId} for user {UserId}", request.DatabaseId, userId);

        return result;
    }

    #endregion
}
