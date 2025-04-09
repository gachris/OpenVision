using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Commands;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.Queries;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Provides operations for interacting with trackable targets secured by X-API-KEY.
/// This service supports retrieval, creation, modification, and deletion of trackable target records.
/// </summary>
public class TrackablesService : ITrackablesService
{
    #region Fields/Consts

    private readonly IMediator _mediator;
    private readonly ILogger<TrackablesService> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TrackablesService"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for dispatching queries and commands.</param>
    /// <param name="logger">The logger instance used for logging information and errors.</param>
    public TrackablesService(
        IMediator mediator, 
        ILogger<TrackablesService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region Methods

    /// <inheritdoc/>
    public async Task<IQueryable<TargetRecordDto>> GetQueryableAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Dispatching GetQueryableTrackableQuery");
        return await _mediator.Send(new GetQueryableTrackableQuery(), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TargetRecordDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching GetTrackablesQuery");
        return await _mediator.Send(new GetTrackablesQuery(), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TargetRecordDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching GetTrackableByIdQuery for target {TargetId}", id);
        return await _mediator.Send(new GetTrackableByIdQuery(id), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TargetRecordDto> CreateAsync(PostTrackableDto postTrackableDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching CreateTrackableCommand");
        return await _mediator.Send(new CreateTrackableCommand(postTrackableDto), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TargetRecordDto> UpdateAsync(Guid id, UpdateTrackableDto updateTrackableDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching UpdateTrackableCommand for target {TargetId}", id);
        return await _mediator.Send(new UpdateTrackableCommand(id, updateTrackableDto), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching DeleteTrackableCommand for target {TargetId}", id);
        return await _mediator.Send(new DeleteTrackableCommand(id), cancellationToken);
    }

    #endregion
}
