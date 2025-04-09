using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.Requests;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Server.Core.Controllers;

/// <summary>
/// API controller for managing databases.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationConsts.BearerPolicy)]
public class DatabasesController : ApiControllerBase
{
    #region Fields/Consts

    private readonly IDatabasesService _databasesService;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabasesController"/> class.
    /// </summary>
    /// <param name="databasesService">The service for interacting with databases.</param>
    /// <param name="mapper">The mapper instance.</param>
    /// <param name="logger">The logger instance for logging information, warnings, and errors.</param>
    public DatabasesController(
        IDatabasesService databasesService,
        IMapper mapper,
        ILogger<DatabasesController> logger) : base(mapper, logger)
    {
        _databasesService = databasesService;
    }

    #region Methods

    /// <summary>
    /// Gets a list of databases matching the provided query parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering the database results.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="IActionResult"/> containing the paged response of databases.</returns>
    [HttpGet]
    [Route("")]
    public virtual async Task<IActionResult> Get([FromQuery] DatabaseBrowserQuery query, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to get databases list with query: {@Query}", query);

            var databaseDtosQueryable = await _databasesService.GetQueryableAsync(cancellationToken);
            var pagedResponse = await GetPagedResponseAsync<DatabaseDto, DatabaseResponse>(databaseDtosQueryable, query, cancellationToken);
            _logger.LogInformation("Returning paged databases list with total records: {TotalRecords}", pagedResponse.TotalRecords);
            return new OkObjectResult(pagedResponse);
        });
    }

    /// <summary>
    /// Gets the database with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the database to retrieve.</param>
    /// <returns>An IActionResult containing the database with the specified ID.</returns>
    [HttpGet]
    [Route("{id:guid}")]
    public virtual async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to get database with id: {Id}", id);
            var databaseDto = await _databasesService.GetAsync(id, cancellationToken);
            _logger.LogInformation("Returning database details for id: {Id}", id);
            return new OkObjectResult(Success(_mapper.Map<DatabaseResponse>(databaseDto)));
        });
    }

    /// <summary>
    /// Creates a new database with the specified properties.
    /// </summary>
    /// <param name="body">The properties of the new database to create.</param>
    /// <returns>An IActionResult containing the newly created database.</returns>
    [HttpPost]
    [Route("")]
    public virtual async Task<IActionResult> Create([FromBody] PostDatabaseRequest body, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to create a new database with description: {Description}", body.Name);
            var createDatabaseDto = _mapper.Map<CreateDatabaseDto>(body);
            var databaseDto = await _databasesService.CreateAsync(createDatabaseDto, cancellationToken);

            var url = Url.Action("Get", new { id = databaseDto.Id });
            ArgumentException.ThrowIfNullOrEmpty(url, nameof(url));

            _logger.LogInformation("Database created with id: {DatabaseId}. Location: {Url}", databaseDto.Id, url);
            var locationUri = new Uri(url, UriKind.Relative);
            return new CreatedResult(locationUri, Success(_mapper.Map<DatabaseResponse>(databaseDto)));
        });
    }

    /// <summary>
    /// Updates the database with the specified ID with the new properties provided.
    /// </summary>
    /// <param name="id">The ID of the database to update.</param>
    /// <param name="body">The new properties of the database to update.</param>
    /// <returns>An IActionResult containing the updated database.</returns>
    [HttpPut]
    [Route("{id:guid}")]
    public virtual async Task<IActionResult> Edit(Guid id, [FromBody] UpdateDatabaseRequest body, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to edit database with id: {Id}", id);
            var updateDatabaseDto = _mapper.Map<UpdateDatabaseDto>(body);
            var databaseDto = await _databasesService.UpdateAsync(id, updateDatabaseDto, cancellationToken);
            _logger.LogInformation("Database with id: {Id} updated successfully.", id);
            return new OkObjectResult(Success(_mapper.Map<DatabaseResponse>(databaseDto)));
        });
    }

    /// <summary>
    /// Deletes the database with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the database to delete.</param>
    /// <returns>An IActionResult indicating the success or failure of the delete operation.</returns>
    [HttpDelete]
    [Route("{id:guid}")]
    public virtual async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to delete database with id: {Id}", id);
            var deleted = await _databasesService.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Database with id: {Id} deleted successfully.", id);
            return new OkObjectResult(Success(deleted));
        });
    }

    #endregion
}