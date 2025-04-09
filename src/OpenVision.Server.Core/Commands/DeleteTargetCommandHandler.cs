using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Repositories.Specifications;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Handles the DeleteTargetCommand and deletes the target.
/// </summary>
public class DeleteTargetCommandHandler : IRequestHandler<DeleteTargetCommand, bool>
{
    #region Fields/Consts

    private readonly IImageTargetsRepository _imageTargetsRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeleteTargetCommandHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTargetCommandHandler"/> class.
    /// </summary>
    /// <param name="imageTargetsRepository">The repository for accessing targets.</param>
    /// <param name="currentUserService">The service for obtaining the current user's identifier.</param>
    /// <param name="logger">The logger for recording informational and error messages.</param>
    public DeleteTargetCommandHandler(
        IImageTargetsRepository imageTargetsRepository,
        ICurrentUserService currentUserService,
        ILogger<DeleteTargetCommandHandler> logger)
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
    public async Task<bool> Handle(DeleteTargetCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        _logger.LogInformation("Deleting target {TargetId} for user {UserId}", request.TargetId, userId);

        var imageTargetForUserSpecification = new ImageTargetForUserSpecification(request.TargetId, userId)
        {
            Includes = { target => target.Database }
        };
        var imageTargets = await _imageTargetsRepository.GetBySpecificationAsync(imageTargetForUserSpecification, cancellationToken);
        var imageTarget = imageTargets.SingleOrDefault();

        if (imageTarget is null)
        {
            _logger.LogWarning("Target {TargetId} not found for user {UserId}", request.TargetId, userId);
            return false;
        }

        var result = await _imageTargetsRepository.RemoveAsync(imageTarget, cancellationToken);
        _logger.LogInformation("Deleted target {TargetId} for user {UserId}", request.TargetId, userId);

        return result;
    }

    #endregion
}
