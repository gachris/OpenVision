using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Contracts;

namespace OpenVision.Server.Core.Commands;

/// <summary>
/// Handles the DeleteTargetCommand and deletes the target.
/// </summary>
public class DeleteTargetCommandHandler : IRequestHandler<DeleteTargetCommand, bool>
{
    #region Fields/Consts

    private readonly IImageTargetsRepository _imageTargetsRepository;
    private readonly ITargetSpecificationFactory _targetSpecificationFactory;
    private readonly ILogger<DeleteTargetCommandHandler> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTargetCommandHandler"/> class.
    /// </summary>
    /// <param name="imageTargetsRepository">The repository for accessing targets.</param>
    /// <param name="targetSpecificationFactory">The factory used to create the appropriate target specification based on the current authentication context.</param>
    /// <param name="logger">The logger for recording informational and error messages.</param>
    public DeleteTargetCommandHandler(
        IImageTargetsRepository imageTargetsRepository,
        ITargetSpecificationFactory targetSpecificationFactory,
        ILogger<DeleteTargetCommandHandler> logger)
    {
        _imageTargetsRepository = imageTargetsRepository;
        _targetSpecificationFactory = targetSpecificationFactory;
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
        var imageTargetForUserSpecification = _targetSpecificationFactory.GetImageTargetSpecification(request.TargetId);
        var imageTargets = await _imageTargetsRepository.GetBySpecificationAsync(imageTargetForUserSpecification, cancellationToken);
        var imageTarget = imageTargets.SingleOrDefault();

        if (imageTarget is null)
        {
            _logger.LogWarning("Target {TargetId} not found", request.TargetId);
            return false;
        }

        var result = await _imageTargetsRepository.RemoveAsync(imageTarget, cancellationToken);
        _logger.LogInformation("Deleted target {TargetId}", request.TargetId);

        return result;
    }

    #endregion
}