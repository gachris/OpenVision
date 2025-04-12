using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Client.Core.Contracts;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Responses;
using OpenVision.Shared.Types;

namespace OpenVision.Client.Core.Mediator.Commands;

/// <summary>
/// Handles the update of an existing target.
/// </summary>
public class UpdateTargetCommandHandler : IRequestHandler<UpdateTargetCommand, ResultDto<TargetResponse>>
{
    private readonly ITargetApiService _targetApiService;
    private readonly ILogger<UpdateTargetCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTargetCommandHandler"/> class.
    /// </summary>
    /// <param name="targetApiService">The target API service used for updating targets.</param>
    /// <param name="logger">The logger instance for diagnostic messages.</param>
    public UpdateTargetCommandHandler(ITargetApiService targetApiService, ILogger<UpdateTargetCommandHandler> logger)
    {
        _targetApiService = targetApiService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the <see cref="UpdateTargetCommand"/> by updating the target identified by its ID.
    /// </summary>
    /// <param name="request">The command containing the target ID and update details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="ResultDto{TargetResponse}"/> containing the updated target on success or error details on failure.
    /// </returns>
    public async Task<ResultDto<TargetResponse>> Handle(UpdateTargetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating target with ID: {TargetId}", request.TargetId);
            var response = await _targetApiService.UpdateAsync(request.TargetId, request.Request, cancellationToken);

            if (response.StatusCode != StatusCode.Success)
            {
                var error = response.Errors.FirstOrDefault()?.Message;
                _logger.LogError("Failed to update target {TargetId}: {Error}", request.TargetId, error);
                return new ResultDto<TargetResponse>(default!, error);
            }

            _logger.LogInformation("Successfully updated target with ID: {TargetId}", request.TargetId);
            return new ResultDto<TargetResponse>(response.Response.Result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while updating target with ID {TargetId}", request.TargetId);
            return new ResultDto<TargetResponse>(default!, "An unexpected error occurred.", ex);
        }
    }
}
