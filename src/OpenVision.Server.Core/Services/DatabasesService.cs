using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Commands;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.Queries;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Provides implementation for database management including creation, retrieval, editing, and deletion.
/// </summary>
public class DatabasesService : IDatabasesService
{
    #region Fields/Consts

    private readonly IMediator _mediator;
    private readonly ILogger<DatabasesService> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabasesService"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    /// <param name="logger">The logger.</param>
    public DatabasesService(
        IMediator mediator,
        ILogger<DatabasesService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region Methods

    /// <inheritdoc/>
    public async Task<IQueryable<DatabaseDto>> GetQueryableAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Dispatching GetQueryableDatabaseQuery");
        return await _mediator.Send(new GetQueryableDatabaseQuery(), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DatabaseDto>> GetAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching GetDatabasesQuery");
        return await _mediator.Send(new GetDatabasesQuery(), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<DatabaseDto?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching GetDatabaseByIdQuery for database {DatabaseId}", id);
        return await _mediator.Send(new GetDatabaseByIdQuery(id), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<DatabaseDto> CreateAsync(CreateDatabaseDto createDatabaseDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching CreateDatabaseCommand");
        return await _mediator.Send(new CreateDatabaseCommand(createDatabaseDto), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<DatabaseDto> UpdateAsync(Guid id, UpdateDatabaseDto updateDatabaseDto, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching UpdateDatabaseCommand for database {DatabaseId}", id);
        return await _mediator.Send(new UpdateDatabaseCommand(id, updateDatabaseDto), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching DeleteDatabaseCommand for database {DatabaseId}", id);
        return await _mediator.Send(new DeleteDatabaseCommand(id), cancellationToken);
    }

    #endregion
}
