using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Contracts;

/// <summary>
/// Provides methods for managing targets including creation, retrieval, editing, activation, deactivation, and deletion.
/// </summary>
public interface ITargetsService
{
    /// <summary>
    /// Retrieves all targets.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An enumerable collection of targets.</returns>
    Task<IQueryable<TargetDto>> GetAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a specific target by its identifier.
    /// </summary>
    /// <param name="id">The target identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The target corresponding to the provided identifier.</returns>
    Task<TargetDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new target.
    /// </summary>
    /// <param name="createTargetDto">The data containing the details for the new target.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The newly created target.</returns>
    Task<TargetDto> CreateAsync(CreateTargetDto createTargetDto, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing target.
    /// </summary>
    /// <param name="id">The target identifier.</param>
    /// <param name="updateTargetDto">The data containing the updated target details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated target.</returns>
    Task<TargetDto> UpdateAsync(Guid id, UpdateTargetDto updateTargetDto, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a target and its associated image asset if present.
    /// </summary>
    /// <param name="id">The target identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A boolean indicating whether the deletion was successful.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
