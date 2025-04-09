using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Repositories.Specifications;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Handles the DeleteTrackableCommand and deletes the target.
/// </summary>
public class DeleteTrackableCommandHandler : IRequestHandler<DeleteTrackableCommand, bool>
{
    #region Fields/Consts

    private readonly IImageTargetsRepository _imageTargetsRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteTrackableCommandHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTargetCommandHandler"/> class.
    /// </summary>
    /// <param name="imageTargetsRepository">The repository for accessing targets.</param>
    /// <param name="currentUserService">The service for obtaining the current user's identifier.</param>
    /// <param name="logger">The logger for recording informational and error messages.</param>
    public DeleteTrackableCommandHandler(
        IImageTargetsRepository imageTargetsRepository,
        ICurrentUserService currentUserService,
        ILogger<DeleteTrackableCommandHandler> logger)
    {
        _imageTargetsRepository = imageTargetsRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles the DeleteTargetCommand request by deleting the target.
    /// </summary>
    /// <param name="request">The command containing the target ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation if needed.</param>
    /// <returns>A boolean value indicating whether the deletion was successful.</returns>
    public async Task<bool> Handle(DeleteTrackableCommand request, CancellationToken cancellationToken)
    {
        var apiKey = _currentUserService.ApiKey;
        _logger.LogInformation("Deleting target {TargetId} for user {UserId}", request.TargetId, apiKey);

        var imageTargetForServerApiKeySpecification = new ImageTargetForServerApiKeySpecification(request.TargetId, apiKey)
        {
            Includes =
            {
                target => target.Database,
                target => target.Database.ApiKeys
            }
        };
        var imageTargets = await _imageTargetsRepository.GetBySpecificationAsync(imageTargetForServerApiKeySpecification, cancellationToken);
        var imageTarget = imageTargets.SingleOrDefault();

        if (imageTarget is null)
        {
            _logger.LogWarning("Target {TargetId} not found for user {UserId}", request.TargetId, apiKey);
            return false;
        }

        var result = await _imageTargetsRepository.RemoveAsync(imageTarget, cancellationToken);
        _logger.LogInformation("Deleted target {TargetId} for user {UserId}", request.TargetId, apiKey);

        return result;
    }

    #endregion
}
