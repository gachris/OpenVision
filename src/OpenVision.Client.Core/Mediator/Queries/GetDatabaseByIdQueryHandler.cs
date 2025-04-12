using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Client.Core.Contracts;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Responses;
using OpenVision.Shared.Types;

namespace OpenVision.Client.Core.Mediator.Queries;

/// <summary>
/// Handles the query for retrieving a single database by its unique identifier.
/// </summary>
public class GetDatabaseByIdQueryHandler : IRequestHandler<GetDatabaseByIdQuery, ResultDto<DatabaseResponse>>
{
    #region Fields

    private readonly IDatabaseApiService _databaseApiService;
    private readonly ILogger<GetDatabaseByIdQueryHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDatabaseByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="databaseApiService">The database API service used for retrieving database details.</param>
    /// <param name="logger">The logger instance used for logging diagnostic messages.</param>
    public GetDatabaseByIdQueryHandler(IDatabaseApiService databaseApiService, ILogger<GetDatabaseByIdQueryHandler> logger)
    {
        _databaseApiService = databaseApiService;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the <see cref="GetDatabaseByIdQuery"/> by retrieving the database corresponding to the specified ID.
    /// </summary>
    /// <param name="request">The query containing the unique identifier of the database to retrieve.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="ResultDto{DatabaseResponse}"/> that contains the retrieved database details on success;
    /// otherwise, an error message and exception information if the operation fails.
    /// </returns>
    public async Task<ResultDto<DatabaseResponse>> Handle(GetDatabaseByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving database with ID: {DatabaseId}", request.DatabaseId);
            var response = await _databaseApiService.GetAsync(request.DatabaseId, cancellationToken);

            if (response.StatusCode != StatusCode.Success)
            {
                var error = response.Errors.FirstOrDefault()?.Message;
                _logger.LogError("Failed to retrieve database with ID {DatabaseId}: {Error}", request.DatabaseId, error);
                return new ResultDto<DatabaseResponse>(default!, error);
            }

            _logger.LogInformation("Successfully retrieved database with ID: {DatabaseId}", request.DatabaseId);
            return new ResultDto<DatabaseResponse>(response.Response.Result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while retrieving database with ID {DatabaseId}", request.DatabaseId);
            return new ResultDto<DatabaseResponse>(default!, "An unexpected error occurred.", ex);
        }
    }

    #endregion
}
