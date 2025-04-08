using OpenVision.Server.Core.Dtos;
using OpenVision.Shared.Requests;

namespace OpenVision.Server.Core.Contracts;

/// <summary>
/// Provides methods for managing databases including creation, retrieval, editing, and deletion.
/// </summary>
public interface IDatabasesService
{
    /// <summary>
    /// Retrieves all databases.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An enumerable collection of databases.</returns>
    Task<IQueryable<DatabaseDto>> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific database by its identifier.
    /// </summary>
    /// <param name="id">The database identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The database corresponding to the provided identifier, or null if not found.</returns>
    Task<DatabaseDto?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new database.
    /// </summary>
    /// <param name="createDatabaseDto">The data containing the details for the new database.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The newly created database.</returns>
    Task<DatabaseDto> CreateAsync(CreateDatabaseDto createDatabaseDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing database.
    /// </summary>
    /// <param name="id">The database identifier.</param>
    /// <param name="updateDatabaseDto">The data containing the updated database details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated database.</returns>
    Task<DatabaseDto> UpdateAsync(Guid id, UpdateDatabaseDto updateDatabaseDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a database.
    /// </summary>
    /// <param name="id">The database identifier.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A boolean indicating whether the deletion was successful.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}