using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Client.Core.Contracts;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Responses;
using OpenVision.Shared.Types;

namespace OpenVision.Client.Core.Mediator.Commands;

/// <summary>
/// Represents a handler for processing the <see cref="UpdateDatabaseCommand"/>, which updates an existing database.
/// </summary>
public class UpdateDatabaseCommandHandler : IRequestHandler<UpdateDatabaseCommand, ResultDto<DatabaseResponse>>
{
    #region Fields/Consts

    private readonly IDatabaseApiService _databaseApiService;
    private readonly ILogger<UpdateDatabaseCommandHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateDatabaseCommandHandler"/> class.
    /// </summary>
    /// <param name="databaseApiService">The database API service used to perform update operations.</param>
    /// <param name="logger">The logger instance for logging diagnostic messages.</param>
    public UpdateDatabaseCommandHandler(IDatabaseApiService databaseApiService, ILogger<UpdateDatabaseCommandHandler> logger)
    {
        _databaseApiService = databaseApiService;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the execution of an <see cref="UpdateDatabaseCommand"/>, updating an existing database.
    /// </summary>
    /// <param name="request">
    /// The update database command containing the database ID and the update request details.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used by other objects or threads to receive notice of cancellation.
    /// </param>
    /// <returns>
    /// A <see cref="ResultDto{DatabaseResponse}"/> indicating the success of the update operation.
    /// On success, the returned data contains the updated <see cref="DatabaseResponse"/>;
    /// on failure, the error and exception (if any) are provided.
    /// </returns>
    public async Task<ResultDto<DatabaseResponse>> Handle(UpdateDatabaseCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating database with ID: {DatabaseId}", request.DatabaseId);
            var response = await _databaseApiService.UpdateAsync(request.DatabaseId, request.Request, cancellationToken);

            if (response.StatusCode != StatusCode.Success)
            {
                var error = response.Errors.FirstOrDefault()?.Message;
                _logger.LogError("Failed to update database {DatabaseId}: {Error}", request.DatabaseId, error);
                return new ResultDto<DatabaseResponse>(default!, error);
            }

            _logger.LogInformation("Successfully updated database with ID: {DatabaseId}", request.DatabaseId);
            return new ResultDto<DatabaseResponse>(response.Response.Result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while updating database with ID: {DatabaseId}", request.DatabaseId);
            return new ResultDto<DatabaseResponse>(default!, "An unexpected error occurred.", ex);
        }
    }

    #endregion
}
