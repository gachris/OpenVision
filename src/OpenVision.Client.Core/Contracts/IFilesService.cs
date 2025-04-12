using OpenVision.Client.Core.Dtos;

namespace OpenVision.Client.Core.Contracts;

/// <summary>
/// Provides operations for handling file-related functionality, including downloading database files from the API.
/// </summary>
public interface IFilesService
{
    /// <summary>
    /// Asynchronously downloads a database file identified by the specified unique identifier from the API.
    /// </summary>
    /// <param name="id">The unique identifier of the database file to be downloaded.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <returns>
    /// A <see cref="DatabaseFileDto"/> object representing the downloaded database file.
    /// </returns>
    Task<DatabaseFileDto> DownloadAsync(Guid id, CancellationToken cancellationToken = default);
}
