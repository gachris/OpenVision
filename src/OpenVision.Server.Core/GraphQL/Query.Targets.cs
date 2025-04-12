using AutoMapper.QueryableExtensions;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.GraphQL.Types;

namespace OpenVision.Server.Core.GraphQL;

/// <summary>
/// GraphQL query resolver for target operations.
/// </summary>
public partial class Query
{
    /// <summary>
    /// Retrieves a paginated, filtered, and sorted list of target entities.
    /// </summary>
    /// <param name="targetsService">The targets service to retrieve target data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IQueryable{Target}"/> representing the target entities.</returns>
    [GraphQLDescription("Retrieves a paginated, filtered, and sorted list of target entities.")]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize(Policy = AuthorizationConsts.BearerPolicy)]
    public virtual async Task<IQueryable<Target>> GetTargets(
        [Service] ITargetsService targetsService,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("GetTargets called.");
            var targetDtoQuery = await targetsService.GetQueryableAsync(cancellationToken);
            _logger.LogInformation("Retrieved {Count} target(s)", await targetDtoQuery.CountAsync(cancellationToken));
            return targetDtoQuery.ProjectTo<Target>(_mapper.ConfigurationProvider);
        });
    }

    /// <summary>
    /// Retrieves a specific target entity by its unique identifier.
    /// Optional filtering is applied based on the provided resolver context.
    /// </summary>
    /// <param name="id">The unique identifier of the target.</param>
    /// <param name="targetsService">The targets service to retrieve target data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The matching <see cref="Target"/> entity if found; otherwise, <c>null</c>.</returns>
    [GraphQLDescription("Retrieves a specific target entity by its unique identifier.")]
    [Authorize(Policy = AuthorizationConsts.BearerPolicy)]
    public virtual async Task<Target?> GetTarget(
        Guid id,
        [Service] ITargetsService targetsService,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("GetTarget called with Id: {TargetId}", id);
            var targetDto = await targetsService.GetAsync(id, cancellationToken);
            if (targetDto == null)
            {
                _logger.LogWarning("No target found with Id: {TargetId}", id);
                return null;
            }
            return _mapper.Map<Target>(targetDto);
        });
    }
}