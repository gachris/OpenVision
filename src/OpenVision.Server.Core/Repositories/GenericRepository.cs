using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.EntityFramework.DbContexts;

namespace OpenVision.Server.Core.Repositories;

/// <summary>
/// A generic repository for common CRUD operations.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public class GenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _applicationContext;
    protected readonly ILogger _logger;

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

    /// <summary>
    /// Retrieves all entities of type <typeparamref name="T"/> as an <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation, containing an <see cref="IQueryable{T}"/> of entities.
    /// </returns>
    public virtual async Task<IQueryable<T>> GetAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all entities of type {EntityType}.", typeof(T).Name);
            var entities = _applicationContext.Set<T>().AsQueryable();
            return await Task.FromResult(entities);
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
}