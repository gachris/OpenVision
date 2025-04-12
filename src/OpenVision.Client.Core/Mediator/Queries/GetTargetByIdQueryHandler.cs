using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Client.Core.Contracts;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Responses;
using OpenVision.Shared.Types;

namespace OpenVision.Client.Core.Mediator.Queries;

/// <summary>
/// Handles the query for retrieving a target by its unique identifier.
/// </summary>
public class GetTargetByIdQueryHandler : IRequestHandler<GetTargetByIdQuery, ResultDto<TargetResponse>>
{
    private readonly ITargetApiService _targetApiService;
    private readonly ILogger<GetTargetByIdQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetTargetByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="targetApiService">The target API service used for target retrieval.</param>
    /// <param name="logger">The logger instance for diagnostic messages.</param>
    public GetTargetByIdQueryHandler(ITargetApiService targetApiService, ILogger<GetTargetByIdQueryHandler> logger)
    {
        _targetApiService = targetApiService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the <see cref="GetTargetByIdQuery"/> by retrieving the target identified by its ID.
    /// </summary>
    /// <param name="request">The query containing the target ID.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="ResultDto{TargetResponse}"/> containing the target details on success or error information on failure.
    /// </returns>
    public async Task<ResultDto<TargetResponse>> Handle(GetTargetByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving target with ID: {TargetId}", request.TargetId);
            var response = await _targetApiService.GetAsync(request.TargetId, cancellationToken);

            if (response.StatusCode != StatusCode.Success)
            {
                var error = response.Errors.FirstOrDefault()?.Message;
                _logger.LogError("Failed to retrieve target with ID {TargetId}: {Error}", request.TargetId, error);
                return new ResultDto<TargetResponse>(default!, error);
            }

            _logger.LogInformation("Successfully retrieved target with ID: {TargetId}", request.TargetId);
            return new ResultDto<TargetResponse>(response.Response.Result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while retrieving target with ID: {TargetId}", request.TargetId);
            return new ResultDto<TargetResponse>(default!, "An unexpected error occurred.", ex);
        }
    }
}
