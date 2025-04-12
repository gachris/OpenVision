using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Client.Core.Contracts;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Responses;
using OpenVision.Shared.Types;

namespace OpenVision.Client.Core.Mediator.Queries;

/// <summary>
/// Handles the query for retrieving a paginated list of targets.
/// </summary>
public class GetTargetsQueryHandler : IRequestHandler<GetTargetsQuery, ResultDto<IPagedResponse<IEnumerable<TargetResponse>>>>
{
    private readonly ITargetApiService _targetApiService;
    private readonly ILogger<GetTargetsQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTargetsQueryHandler"/> class.
    /// </summary>
    /// <param name="targetApiService">The target API service used for retrieving targets.</param>
    /// <param name="logger">The logger instance for diagnostic messages.</param>
    public GetTargetsQueryHandler(ITargetApiService targetApiService, ILogger<GetTargetsQueryHandler> logger)
    {
        _targetApiService = targetApiService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the <see cref="GetTargetsQuery"/> by retrieving a paginated list of targets.
    /// </summary>
    /// <param name="request">The query containing pagination and filtering parameters.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="ResultDto{IPagedResponse{IEnumerable{TargetResponse}}}"/> containing the target list on success or error details on failure.
    /// </returns>
    public async Task<ResultDto<IPagedResponse<IEnumerable<TargetResponse>>>> Handle(GetTargetsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving targets: Page={Page}, Size={Size}", request.Query.Page, request.Query.Size);
            var response = await _targetApiService.GetAsync(request.Query, cancellationToken);

            if (response.StatusCode != StatusCode.Success)
            {
                var error = response.Errors.FirstOrDefault()?.Message;
                _logger.LogError("Failed to retrieve targets: {Error}", error);
                return new ResultDto<IPagedResponse<IEnumerable<TargetResponse>>>(default!, error);
            }

            _logger.LogInformation("Successfully retrieved targets.");
            return new ResultDto<IPagedResponse<IEnumerable<TargetResponse>>>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while retrieving targets.");
            return new ResultDto<IPagedResponse<IEnumerable<TargetResponse>>>(default!, "An unexpected error occurred.", ex);
        }
    }
}