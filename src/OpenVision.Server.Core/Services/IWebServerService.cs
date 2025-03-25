using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Defines the interface for interacting with web server-related operations.
/// </summary>
public interface IWebServerService
{
    /// <summary>
    /// Retrieves a collection of trackable entities from the web server.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>A task representing the asynchronous operation, returning GetAllTrackablesResponse.</returns>
    Task<GetAllTrackablesResponse> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific trackable entity from the web server based on its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the trackable entity.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>A task representing the asynchronous operation, returning TrackableRetrieveResponse.</returns>
    Task<TrackableRetrieveResponse> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new trackable entity on the web server.
    /// </summary>
    /// <param name="body">The request body containing details of the new trackable entity.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>A task representing the asynchronous operation, returning PostTrackableResponse.</returns>
    Task<PostTrackableResponse> CreateAsync(PostTrackableRequest body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing trackable entity on the web server.
    /// </summary>
    /// <param name="id">The unique identifier of the trackable entity to update.</param>
    /// <param name="body">The request body containing updated details of the trackable entity.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>A task representing the asynchronous operation, returning IResponseMessage.</returns>
    Task<IResponseMessage> UpdateAsync(Guid id, UpdateTrackableRequest body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing trackable entity from the web server.
    /// </summary>
    /// <param name="id">The unique identifier of the trackable entity to delete.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>A task representing the asynchronous operation, returning IResponseMessage.</returns>
    Task<IResponseMessage> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
