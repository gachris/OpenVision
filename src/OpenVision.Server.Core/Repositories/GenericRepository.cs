using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.EntityFramework.DbContexts;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Repositories.Specifications;

namespace OpenVision.Server.Core.Repositories;

/// <summary>
/// A generic repository for common CRUD operations.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    #region Fields/Consts

    protected readonly ApplicationDbContext _applicationContext;
    protected readonly ILogger _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class using the provided application database context.
    /// </summary>
    /// <param name="applicationContext">The application database context.</param>
    /// <param name="logger">The logger instance.</param>
    public GenericRepository(ApplicationDbContext applicationContext, ILogger logger)
    {
        _applicationContext = applicationContext;
        _logger = logger;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GenericRepository{T}"/> class using a context factory.
    /// </summary>
    /// <param name="applicationContextPool">The database context factory.</param>
    /// <param name="logger">The logger instance.</param>
    public GenericRepository(IDbContextFactory<ApplicationDbContext> applicationContextPool, ILogger logger)
    {
        _applicationContext = applicationContextPool.CreateDbContext();
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Retrieves all entities of type <typeparamref name="T"/> as an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, containing an <see cref="IEnumerable{T}"/> of entities.
    /// </returns>
    public virtual async Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all entities of type {EntityType}.", typeof(T).Name);
            var entities = _applicationContext.Set<T>().AsQueryable();
            return await entities.ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving entities of type {EntityType}.", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Creates a new entity in the database.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a boolean result indicating whether the creation was successful.
    /// </returns>
    public virtual async Task<bool> CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating a new entity of type {EntityType}.", typeof(T).Name);
            await _applicationContext.Set<T>().AddAsync(entity, cancellationToken);
            var affectedRows = await _applicationContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Created entity of type {EntityType}. Affected rows: {AffectedRows}.", typeof(T).Name, affectedRows);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating entity of type {EntityType}.", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing entity in the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a boolean result indicating whether the update was successful.
    /// </returns>
    public virtual async Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating entity of type {EntityType}.", typeof(T).Name);
            _applicationContext.Set<T>().Update(entity);
            var affectedRows = await _applicationContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Updated entity of type {EntityType}. Affected rows: {AffectedRows}.", typeof(T).Name, affectedRows);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity of type {EntityType}.", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Removes an existing entity from the database.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task representing the asynchronous operation, with a boolean result indicating whether the removal was successful.
    /// </returns>
    public virtual async Task<bool> RemoveAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Removing entity of type {EntityType}.", typeof(T).Name);
            _applicationContext.Set<T>().Remove(entity);
            var affectedRows = await _applicationContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Removed entity of type {EntityType}. Affected rows: {AffectedRows}.", typeof(T).Name, affectedRows);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing entity of type {EntityType}.", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Retrieves entities of type <typeparamref name="T"/> from the database that satisfy the provided specification.
    /// </summary>
    /// <param name="specification">An instance of <see cref="ISpecification{T}"/> defining the filtering criteria for the query.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns an enumerable collection of entities matching the specification.
    /// </returns>
    public IQueryable<T> GetQueryableBySpecification(ISpecification<T> specification)
    {
        try
        {
            _logger.LogInformation("Retrieving entities of type {EntityType} by specification.", typeof(T).Name);

            var query = _applicationContext.Set<T>().AsQueryable();
            query = specification.Apply(query);

            _logger.LogInformation("Retrieved {ResultCount} entities of type {EntityType} by specification.", query.Count(), typeof(T).Name);

            return query;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving entities of type {EntityType} by specification.", typeof(T).Name);
            throw;
        }
    }

    /// <summary>
    /// Retrieves entities of type <typeparamref name="T"/> from the database that satisfy the provided specification.
    /// </summary>
    /// <param name="specification">An instance of <see cref="ISpecification{T}"/> defining the filtering criteria for the query.</param>
    /// <returns>
    /// A task representing the asynchronous operation that returns an enumerable collection of entities matching the specification.
    /// </returns>
    public async Task<IEnumerable<T>> GetBySpecificationAsync(ISpecification<T> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving entities of type {EntityType} by specification.", typeof(T).Name);

            var query = _applicationContext.Set<T>().AsQueryable();
            query = specification.Apply(query);

            var results = await query.ToListAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("Retrieved {ResultCount} entities of type {EntityType} by specification.", results.Count, typeof(T).Name);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving entities of type {EntityType} by specification.", typeof(T).Name);
            throw;
        }
    }

    #endregion
}