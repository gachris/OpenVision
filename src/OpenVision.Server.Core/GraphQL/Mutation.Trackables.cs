using HotChocolate.Authorization;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.GraphQL.Inputs;
using OpenVision.Server.Core.GraphQL.Payloads;
using OpenVision.Server.Core.GraphQL.Types;

namespace OpenVision.Server.Core.GraphQL;

/// <summary>
/// GraphQL mutation resolver for target operations.
/// </summary>
public partial class Mutation
{
    /// <summary>
    /// Creates a new target.
    /// </summary>
    /// <param name="input">An object containing the target creation details.</param>
    /// <param name="targetsService">The service that manages target operations.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A payload containing the created target.</returns>
    [GraphQLDescription("Creates a new target.")]
    [Authorize(Policy = AuthorizationConsts.ServerApiKeyPolicy)]
    public virtual async Task<CreateTargetPayload> CreateTrackableAsync(
        PostTrackableInput input,
        [Service] ITargetsService targetsService,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("CreateTrackableAsync called.");
            var targetDto = await targetsService.CreateAsync(_mapper.Map<CreateTargetDto>(input), cancellationToken);

            return new CreateTargetPayload
            {
                Target = _mapper.Map<TargetRecordModel>(targetDto)
            };
        });
    }

    /// <summary>
    /// Updates an existing target identified by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the target to update.</param>
    /// <param name="input">An object containing the updated target details. Each field is optional.</param>
    /// <param name="targetsService">The service that manages target operations.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A payload containing the updated target.</returns>
    [GraphQLDescription("Updates an existing target identified by its unique identifier.")]
    [Authorize(Policy = AuthorizationConsts.ServerApiKeyPolicy)]
    public virtual async Task<UpdateTargetPayload> UpdateTrackableAsync(
        Guid id,
        UpdateTrackableInput input,
        [Service] ITargetsService targetsService,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("UpdateTrackableAsync called for Id: {TargetId}", id);
            var targetDto = await targetsService.UpdateAsync(id, _mapper.Map<UpdateTargetDto>(input), cancellationToken);

            return new UpdateTargetPayload
            {
                Target = _mapper.Map<TargetRecordModel>(targetDto)
            };
        });
    }

    /// <summary>
    /// Deletes an existing target identified by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the target to delete.</param>
    /// <param name="targetsService">The service that manages target operations.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A payload indicating whether the deletion was successful.</returns>
    [GraphQLDescription("Deletes an existing target identified by its unique identifier.")]
    [Authorize(Policy = AuthorizationConsts.ServerApiKeyPolicy)]
    public virtual async Task<DeleteTargetPayload> DeleteTrackableAsync(
        Guid id,
        [Service] ITargetsService targetsService,
        CancellationToken cancellationToken)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("DeleteTrackableAsync called for Id: {TargetId}", id);
            var deleted = await targetsService.DeleteAsync(id, cancellationToken);

            return new DeleteTargetPayload
            {
                Success = deleted
            };
        });
    }
}
