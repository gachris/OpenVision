using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Client.Core.Contracts;
using OpenVision.Client.Core.Dtos;
using OpenVision.Shared.Responses;
using OpenVision.Shared.Types;

namespace OpenVision.Client.Core.Mediator.Commands;

/// <summary>
/// Handles the creation of a new target.
/// </summary>
public class CreateTargetCommandHandler : IRequestHandler<CreateTargetCommand, ResultDto<TargetResponse>>
{
    private readonly ITargetApiService _targetApiService;
    private readonly ILogger<CreateTargetCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateTargetCommandHandler"/> class.
    /// </summary>
    /// <param name="targetApiService">The target API service used to create the target.</param>
    /// <param name="logger">The logger instance for diagnostic messages.</param>
    public CreateTargetCommandHandler(ITargetApiService targetApiService, ILogger<CreateTargetCommandHandler> logger)
    {
        _targetApiService = targetApiService;
        _logger = logger;
    }

    /// <summary>
    /// Handles the <see cref="CreateTargetCommand"/> by creating a new target.
    /// </summary>
    /// <param name="request">The command containing target creation details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A <see cref="ResultDto{TargetResponse}"/> containing the created target on success or error details on failure.
    /// </returns>
    public async Task<ResultDto<TargetResponse>> Handle(CreateTargetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating target with Name: {Name}", request.Request.Name);
            var response = await _targetApiService.CreateAsync(request.Request, cancellationToken);

            if (response.StatusCode != StatusCode.Success)
            {
                var error = response.Errors.FirstOrDefault()?.Message;
                _logger.LogError("Failed to create target: {Error}", error);
                return new ResultDto<TargetResponse>(default!, error);
            }

            _logger.LogInformation("Successfully created target with ID: {Id}", response.Response.Result.Id);
            return new ResultDto<TargetResponse>(response.Response.Result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating target.");
            return new ResultDto<TargetResponse>(default!, "An unexpected error occurred.", ex);
        }
    }
}
