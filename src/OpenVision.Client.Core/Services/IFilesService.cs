using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Services;

/// <summary>
/// Defines the interface for the files service, which is responsible for handling requests and responses related to files.
/// </summary>
public interface IFilesService
{
    /// <summary>
    /// Downloads the database file associated with the specified identifier from the API.
    /// </summary>
    /// <param name="id">The unique identifier of the database record.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the request.</param>
    /// <returns>A response message containing the downloaded file as a <see cref="DownloadFileResult"/> object.</returns>
    Task<IResponseMessage<DownloadFileResult>> DownloadAsync(Guid id, CancellationToken cancellationToken = default);
}