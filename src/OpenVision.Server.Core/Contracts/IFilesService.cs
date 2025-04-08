using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Contracts;

/// <summary>
/// Defines the interface for the files service, responsible for handling file download operations.
/// </summary>
public interface IFilesService
{
    /// <summary>
    /// Downloads the file associated with the specified database record.
    /// The file contains serialized data representing the image targets linked to the database.
    /// </summary>
    /// <param name="id">The unique identifier of the database record.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the download operation.</param>
    /// <returns>
    /// A response message containing a <see cref="DatabaseFileDto"/> with the file name, contents, and content type.
    /// </returns>
    Task<DatabaseFileDto> GetDatabaseFileAsync(Guid id, CancellationToken cancellationToken = default);
}
