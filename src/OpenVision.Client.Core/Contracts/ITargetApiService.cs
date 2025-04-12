using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Contracts;

/// <summary>
/// Provides an interface for interacting with the targets API endpoints.
/// </summary>
public interface ITargetApiService
{
    /// <summary>
    /// Retrieves a collection of targets that match the specified query parameters.
    /// </summary>
    /// <param name="query">The query parameters to use when retrieving the targets.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the collection of targets that match the specified query parameters.</returns>
    Task<IPagedResponse<IEnumerable<TargetResponse>>> GetAsync(TargetBrowserQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the target with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the target to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the target with the specified identifier.</returns>
    Task<IResponseMessage<TargetResponse>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new target.
    /// </summary>
    /// <param name="body">The request body containing the information needed to create the target.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the new target identifier.</returns>
    Task<IResponseMessage<TargetResponse>> CreateAsync(PostTargetRequest body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing target.
    /// </summary>
    /// <param name="id">The unique identifier of the target to update.</param>
    /// <param name="body">The request body containing the information needed to update the target.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<IResponseMessage<TargetResponse>> UpdateAsync(Guid id, UpdateTargetRequest body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the target with the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the target to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task<IResponseMessage<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}