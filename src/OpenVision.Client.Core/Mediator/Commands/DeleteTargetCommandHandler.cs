using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Client.Core.Contracts;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Types;

namespace OpenVision.Client.Core.Mediator.Commands;

/// <summary>
/// Handles the deletion of a target.
/// </summary>
public class DeleteTargetCommandHandler : IRequestHandler<DeleteTargetCommand, ResultDto<bool>>
{
    private readonly ITargetApiService _targetApiService;
    private readonly ILogger<DeleteTargetCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTargetCommandHandler"/> class.
    /// </summary>
    /// <param name="targetApiService">The target API service used to delete targets.</param>
    /// <param name="logger">The logger instance for diagnostic messages.</param>
    public DeleteTargetCommandHandler(ITargetApiService targetApiService, ILogger<DeleteTargetCommandHandler> logger)
    {
        _targetApiService = targetApiService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the <see cref="DeleteTargetCommand"/> by deleting the target identified by its ID.
    /// </summary>
    /// <param name="request">The command containing the target ID to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="ResultDto{Boolean}"/> indicating whether the deletion was successful or error details if it failed.
    /// </returns>
    public async Task<ResultDto<bool>> Handle(DeleteTargetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting target with ID: {TargetId}", request.TargetId);
            var response = await _targetApiService.DeleteAsync(request.TargetId, cancellationToken);

            if (response.StatusCode != StatusCode.Success)
            {
                var error = response.Errors.FirstOrDefault()?.Message;
                _logger.LogError("Failed to delete target {TargetId}: {Error}", request.TargetId, error);
                return new ResultDto<bool>(default!, error);
            }

            _logger.LogInformation("Successfully deleted target with ID: {TargetId}", request.TargetId);
            return new ResultDto<bool>(response.Response.Result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting target with ID: {TargetId}", request.TargetId);
            return new ResultDto<bool>(default!, "An unexpected error occurred.", ex);
        }
    }
}
