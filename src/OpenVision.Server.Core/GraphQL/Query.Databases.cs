using AutoMapper.QueryableExtensions;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.GraphQL.Types;

namespace OpenVision.Server.Core.GraphQL;

/// <summary>
/// GraphQL query resolver for database operations.
/// </summary>
public partial class Query
{
    /// <summary>
    /// Retrieves a paginated, filtered, and sorted list of database entities.
    /// </summary>
    /// <param name="databasesService">The databases service to retrieve database data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IQueryable{Database}"/> representing the database entities.</returns>
    [GraphQLDescription("Retrieves a paginated, filtered, and sorted list of database entities.")]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize(Policy = AuthorizationConsts.BearerPolicy)]
    public virtual async Task<IQueryable<Database>> GetDatabases(
        [Service] IDatabasesService databasesService,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("GetDatabases called.");
            var databaseDtoQuery = await databasesService.GetQueryableAsync(cancellationToken);
            _logger.LogInformation("Retrieved {Count} database(s)", await databaseDtoQuery.CountAsync(cancellationToken));
            return databaseDtoQuery.ProjectTo<Database>(_mapper.ConfigurationProvider);
        });
    }

    /// <summary>
    /// Retrieves a specific database entity by its unique identifier.
    /// Optional filtering is applied based on the provided resolver context.
    /// </summary>
    /// <param name="id">The unique identifier of the database.</param>
    /// <param name="databasesService">The databases service to retrieve database data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The matching <see cref="Database"/> entity if found; otherwise, <c>null</c>.</returns>
    [GraphQLDescription("Retrieves a specific database entity by its unique identifier.")]
    [Authorize(Policy = AuthorizationConsts.BearerPolicy)]
    public virtual async Task<Database?> GetDatabase(
        Guid id,
        [Service] IDatabasesService databasesService,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("GetDatabase called with Id: {DatabaseId}", id);
            var databaseDto = await databasesService.GetAsync(id, cancellationToken);
            if (databaseDto == null)
            {
                _logger.LogWarning("No database found with Id: {DatabaseId}", id);
                return null;
            }
            return _mapper.Map<Database>(databaseDto);
        });
    }
}