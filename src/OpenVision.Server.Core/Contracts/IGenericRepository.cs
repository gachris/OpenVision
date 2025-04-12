using OpenVision.Server.Core.Repositories.Specifications;

namespace OpenVision.Server.Core.Contracts;

/// <summary>
/// Provides a contract for common CRUD operations with generic support.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Retrieves all entities of type <typeparamref name="T"/> as an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing an <see cref="IEnumerable{T}"/> of entities.
    /// </returns>
    Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new entity in the database.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a boolean result indicating whether the creation was successful.
    /// </returns>
    Task<bool> CreateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a boolean result indicating whether the update was successful.
    /// </returns>
    Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an existing entity from the database.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a boolean result indicating whether the removal was successful.
    /// </returns>
    Task<bool> RemoveAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves entities of type <typeparamref name="T"/> from the database that satisfy the provided specification.
    /// </summary>
    /// <param name="specification">An instance of <see cref="ISpecification{T}"/> defining the filtering criteria for the query.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns an enumerable collection of entities matching the specification.
    /// </returns>
    IQueryable<T> GetQueryableBySpecification(ISpecification<T> specification);

    /// <summary>
    /// Retrieves entities of type <typeparamref name="T"/> from the database that satisfy the provided specification.
    /// </summary>
    /// <param name="specification">An instance of <see cref="ISpecification{T}"/> defining the filtering criteria for the query.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns an enumerable collection of entities matching the specification.
    /// </returns>
    Task<IEnumerable<T>> GetBySpecificationAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
}