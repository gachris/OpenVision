using MediatR;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Commands;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;

namespace OpenVision.Server.Core.Services;

/// <summary>
/// Service class that provides functionality to download files.
/// </summary>
public class FilesService : IFilesService
{
    #region Fields/Consts

    private readonly IMediator _mediator;
    private readonly ILogger<TargetsService> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the FilesService class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    /// <param name="logger">The logger instance for logging information and errors.</param>
    public FilesService(IMediator mediator, ILogger<TargetsService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    #region Methods

    /// <inheritdoc/>
    public async Task<DatabaseFileDto> GetDatabaseFileAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching DownloadDatabaseFileCommand");
        return await _mediator.Send(new GetDatabaseFileCommand(id), cancellationToken);
    }

    #endregion
}