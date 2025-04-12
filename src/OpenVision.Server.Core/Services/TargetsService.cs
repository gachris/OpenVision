using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Commands;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.Queries;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Provides implementation for target management including creation, retrieval, editing, and deletion.
/// </summary>
public class TargetsService : ITargetsService
{
    #region Fields/Consts

    private readonly IMediator _mediator;
    private readonly ILogger<TargetsService> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetsService"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    /// <param name="logger">The logger instance for logging information and errors.</param>
    public TargetsService(
        IMediator mediator,
        ILogger<TargetsService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region Methods

    /// <inheritdoc/>
    public async Task<IQueryable<TargetDto>> GetQueryableAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching GetQueryableTargetQuery");
        return await _mediator.Send(new GetQueryableTargetQuery(), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TargetDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching GetTargetsQuery");
        return await _mediator.Send(new GetTargetsQuery(), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TargetDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching GetTargetByIdQuery for target {TargetId}", id);
        return await _mediator.Send(new GetTargetByIdQuery(id), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TargetDto> CreateAsync(CreateTargetDto createTargetDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching CreateTargetCommand");
        return await _mediator.Send(new CreateTargetCommand(createTargetDto), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<TargetDto> UpdateAsync(Guid id, UpdateTargetDto updateTargetDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching UpdateTargetCommand for target {TargetId}", id);
        return await _mediator.Send(new UpdateTargetCommand(id, updateTargetDto), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching DeleteTargetCommand for target {TargetId}", id);
        return await _mediator.Send(new DeleteTargetCommand(id), cancellationToken);
    }

    #endregion
}
