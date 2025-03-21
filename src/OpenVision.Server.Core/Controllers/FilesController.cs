using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Services;

namespace OpenVision.Server.Core.Controllers;

/// <summary>
/// API controller for managing files.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationConsts.BearerPolicy)]
public class FilesController : ControllerBase
{
    #region Fields/Consts

    /// <summary>
    /// The service for interacting with files.
    /// </summary>
    private readonly IFilesService _filesService;

    /// <summary>
    /// The logger instance for logging information, warnings, and errors.
    /// </summary>
    private readonly ILogger<FilesController> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="FilesController"/> class.
    /// </summary>
    /// <param name="filesService">The service for interacting with files.</param>
    /// <param name="logger">The logger instance for logging information, warnings, and errors.</param>
    public FilesController(IFilesService filesService, ILogger<FilesController> logger)
    {
        _filesService = filesService;
        _logger = logger;
    }

    /// <summary>
    /// Downloads the file with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the file to download.</param>
    /// <returns>An IActionResult containing the contents of the downloaded file.</returns>
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _filesService.DownloadAsync(id, CancellationToken.None);
        return File(result.Response.Result.FileContents, result.Response.Result.ContentType, result.Response.Result.Filename);
    }
}
