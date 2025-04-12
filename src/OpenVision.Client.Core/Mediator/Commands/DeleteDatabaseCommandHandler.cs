using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Client.Core.Contracts;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Types;

namespace OpenVision.Client.Core.Mediator.Commands;

/// <summary>
/// Handles the deletion of a database by processing a <see cref="DeleteDatabaseCommand"/>.
/// </summary>
public class DeleteDatabaseCommandHandler : IRequestHandler<DeleteDatabaseCommand, ResultDto<bool>>
{
    #region Fields

    private readonly IDatabaseApiService _databaseApiService;
    private readonly ILogger<DeleteDatabaseCommandHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteDatabaseCommandHandler"/> class.
    /// </summary>
    /// <param name="databaseApiService">The database API service used to perform deletion operations.</param>
    /// <param name="logger">The logger instance used to log diagnostic messages.</param>
    public DeleteDatabaseCommandHandler(IDatabaseApiService databaseApiService, ILogger<DeleteDatabaseCommandHandler> logger)
    {
        _databaseApiService = databaseApiService;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the deletion of a database.
    /// </summary>
    /// <param name="request">
    /// A <see cref="DeleteDatabaseCommand"/> containing the unique identifier of the database to be deleted.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="ResultDto{T}"/> of type <see cref="bool"/> indicating whether the deletion was successful.
    /// On failure, the error message and optional exception information will be provided.
    /// </returns>
    public async Task<ResultDto<bool>> Handle(DeleteDatabaseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting database with ID: {DatabaseId}", request.DatabaseId);
            var response = await _databaseApiService.DeleteAsync(request.DatabaseId, cancellationToken);

            if (response.StatusCode != StatusCode.Success)
            {
                var error = response.Errors.FirstOrDefault()?.Message;
                _logger.LogError("Failed to delete database {DatabaseId}: {Error}", request.DatabaseId, error);
                return new ResultDto<bool>(default!, error);
            }

            _logger.LogInformation("Successfully deleted database with ID: {DatabaseId}", request.DatabaseId);
            return new ResultDto<bool>(response.Response.Result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting database with ID: {DatabaseId}", request.DatabaseId);
            return new ResultDto<bool>(default!, "An unexpected error occurred.", ex);
        }
    }

    #endregion
}
