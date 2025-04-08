using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Contracts;
using OpenVision.Shared.Responses;

namespace OpenVision.Server.Core.Controllers;

/// <summary>
/// API controller for managing files.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationConsts.BearerPolicy)]
public class FilesController : ApiControllerBase
{
    #region Fields/Consts

    private readonly IFilesService _filesService;
    private readonly IMapper _mapper;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="FilesController"/> class.
    /// </summary>
    /// <param name="filesService">The service for interacting with files.</param>
    /// <param name="mapper">The mapper instance.</param>
    /// <param name="logger">The logger instance.</param>
    public FilesController(
        IFilesService filesService,
        IMapper mapper,
        ILogger<FilesController> logger) : base(logger)
    {
        _filesService = filesService;
        _mapper = mapper;
    }

    #region Methods

    /// <summary>
    /// Downloads the file with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the file to download.</param>
    /// <returns>An IActionResult containing the contents of the downloaded file.</returns>
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to download database file for database id: {DatabaseId}", id);

            var databaseFileDto = await _filesService.GetDatabaseFileAsync(id, CancellationToken.None);

            _logger.LogInformation("Successfully retrieved database file for database id: {DatabaseId}", id);
            return File(databaseFileDto.FileContents, databaseFileDto.ContentType, databaseFileDto.Filename);
        });
    } 

    #endregion
}