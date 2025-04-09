using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Contracts;

/// <summary>
/// Provides methods for managing trackable records, including creation, retrieval, editing, and deletion.
/// </summary>
public interface ITrackablesService
{
    /// <summary>
    /// Gets a queryable collection of trackable record DTOs for further composition (e.g., filtering, sorting, paging).
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>An <see cref="IQueryable{TargetRecordDto}"/> of trackable record DTOs.</returns>
    Task<IQueryable<TargetRecordDto>> GetQueryableAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves all trackable records.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An enumerable collection of trackable records.</returns>
    Task<IEnumerable<TargetRecordDto>> GetAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a specific trackable record by its identifier.
    /// </summary>
    /// <param name="id">The trackable record identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The trackable record corresponding to the provided identifier.</returns>
    Task<TargetRecordDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Creates a new trackable record.
    /// </summary>
    /// <param name="postTrackableDto">The data containing the details for the new trackable record.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The newly created trackable record.</returns>
    Task<TargetRecordDto> CreateAsync(PostTrackableDto postTrackableDto, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing trackable record.
    /// </summary>
    /// <param name="id">The trackable record identifier.</param>
    /// <param name="updateTrackableDto">The data containing the updated details of the trackable record.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated trackable record.</returns>
    Task<TargetRecordDto> UpdateAsync(Guid id, UpdateTrackableDto updateTrackableDto, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a trackable record.
    /// </summary>
    /// <param name="id">The trackable record identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A boolean indicating whether the deletion was successful.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
