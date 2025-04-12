using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Client.Core.Contracts;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Responses;
using OpenVision.Shared.Types;

namespace OpenVision.Client.Core.Mediator.Queries;

/// <summary>
/// Handles the query for retrieving a paginated list of databases.
/// </summary>
public class GetDatabasesQueryHandler : IRequestHandler<GetDatabasesQuery, ResultDto<IPagedResponse<IEnumerable<DatabaseResponse>>>>
{
    #region Fields

    private readonly IDatabaseApiService _databaseApiService;
    private readonly ILogger<GetDatabasesQueryHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDatabasesQueryHandler"/> class.
    /// </summary>
    /// <param name="databaseApiService">The database API service used to retrieve databases.</param>
    /// <param name="logger">The logger used to log diagnostic messages.</param>
    public GetDatabasesQueryHandler(IDatabaseApiService databaseApiService, ILogger<GetDatabasesQueryHandler> logger)
    {
        _databaseApiService = databaseApiService;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the <see cref="GetDatabasesQuery"/> by retrieving a paginated list of databases based on the specified query parameters.
    /// </summary>
    /// <param name="request">
    /// The query containing pagination and filtering parameters for the databases.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that can be used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ResultDto{IPagedResponse{IEnumerable{DatabaseResponse}}}"/> containing the paginated list of databases on success, 
    /// or error information if the operation fails.
    /// </returns>
    public async Task<ResultDto<IPagedResponse<IEnumerable<DatabaseResponse>>>> Handle(GetDatabasesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving databases: Page={Page}, Size={Size}, Name='{Name}'",
                request.Query.Page, request.Query.Size, request.Query.Name);

            var response = await _databaseApiService.GetAsync(request.Query, cancellationToken);

            if (response.StatusCode != StatusCode.Success)
            {
                var error = response.Errors.FirstOrDefault()?.Message;
                _logger.LogError("Failed to retrieve databases: {Error}", error);
                return new ResultDto<IPagedResponse<IEnumerable<DatabaseResponse>>>(default!, error);
            }

            _logger.LogInformation("Successfully retrieved databases.");
            return new ResultDto<IPagedResponse<IEnumerable<DatabaseResponse>>>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while retrieving databases.");
            return new ResultDto<IPagedResponse<IEnumerable<DatabaseResponse>>>(default!, "An unexpected error occurred.", ex);
        }
    }

    #endregion
}
