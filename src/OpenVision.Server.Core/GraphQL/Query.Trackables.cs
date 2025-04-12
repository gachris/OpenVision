using AutoMapper.QueryableExtensions;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.GraphQL.Types;

namespace OpenVision.Server.Core.GraphQL;

/// <summary>
/// GraphQL query resolver for trackable operations.
/// </summary>
public partial class Query
{
    /// <summary>
    /// Retrieves a paginated, filtered, and sorted list of trackable target records.
    /// </summary>
    /// <param name="targetsService">The targets service to retrieve target data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// An <see cref="IQueryable{TargetRecordModel}"/> representing the trackable target records.
    /// </returns>
    [GraphQLDescription("Retrieves a paginated, filtered, and sorted list of trackable target records.")]
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize(Policy = AuthorizationConsts.ServerApiKeyPolicy)]
    public virtual async Task<IQueryable<TargetRecordModel>> GetTrackables(
        [Service] ITargetsService targetsService,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("{Method} called.", nameof(GetTrackables));
            var targetDtoQuery = await targetsService.GetQueryableAsync(cancellationToken);
            _logger.LogInformation("Retrieved {Count} trackable target(s)", await targetDtoQuery.CountAsync(cancellationToken));
            return targetDtoQuery.ProjectTo<TargetRecordModel>(_mapper.ConfigurationProvider);
        });
    }

    /// <summary>
    /// Retrieves a specific trackable target record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the trackable target.</param>
    /// <param name="targetsService">The targets service to retrieve target data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// The matching <see cref="TargetRecordModel"/> if found; otherwise, <c>null</c>.
    /// </returns>
    [GraphQLDescription("Retrieves a specific trackable target record by its unique identifier.")]
    [Authorize(Policy = AuthorizationConsts.ServerApiKeyPolicy)]
    public virtual async Task<TargetRecordModel?> GetTrackable(
        Guid id,
        [Service] ITargetsService targetsService,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("GetTrackable called with Id: {TargetId}", id);
            var targetDto = await targetsService.GetAsync(id, cancellationToken);
            if (targetDto == null)
            {
                _logger.LogWarning("No trackable target found with Id: {TargetId}", id);
                return null;
            }
            return _mapper.Map<TargetRecordModel>(targetDto);
        });
    }
}