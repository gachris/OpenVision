using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Contracts;

/// <summary>
/// Provides an interface for interacting with the databases API endpoints.
/// </summary>
public interface IDatabaseApiService
{
    /// <summary>
    /// Retrieves a collection of database records from the API based on the specified query.
    /// </summary>
    /// <param name="query">The query parameters used to filter, sort, and paginate the results.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>A paged response containing a collection of <see cref="DatabaseResponse"/> objects.</returns>
    Task<IPagedResponse<IEnumerable<DatabaseResponse>>> GetAsync(DatabaseBrowserQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific database record from the API based on the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the database record.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>A response message containing a single <see cref="DatabaseResponse"/> object.</returns>
    Task<IResponseMessage<DatabaseResponse>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new database record in the API based on the specified request body.
    /// </summary>
    /// <param name="body">The request body containing the details of the new database record.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>A response message indicating the success or failure of the request. The task result contains the new database identifier.</returns>
    Task<IResponseMessage<DatabaseResponse>> CreateAsync(PostDatabaseRequest body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing database record in the API based on the specified identifier and request body.
    /// </summary>
    /// <param name="id">The unique identifier of the database record to update.</param>
    /// <param name="body">The request body containing the updated details of the database record.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>A response message indicating the success or failure of the request.</returns>
    Task<IResponseMessage<DatabaseResponse>> UpdateAsync(Guid id, UpdateDatabaseRequest body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing database record from the API based on the specified identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the database record to delete.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>A response message indicating the success or failure of the request.</returns>
    Task<IResponseMessage<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}