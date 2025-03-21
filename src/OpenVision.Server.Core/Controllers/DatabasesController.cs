using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Services;
using OpenVision.Shared.Requests;
using OpenVision.Web.Core.Filters;

namespace OpenVision.Server.Core.Controllers;

/// <summary>
/// API controller for managing databases.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationConsts.BearerPolicy)]
public class DatabasesController : ControllerBase
{
    #region Fields/Consts

    /// <summary>
    /// The logger instance for logging information, warnings, and errors.
    /// </summary>
    private readonly ILogger<DatabasesController> _logger;

    /// <summary>
    /// The service for interacting with databases.
    /// </summary>
    private readonly IDatabasesService _databasesService;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabasesController"/> class.
    /// </summary>
    /// <param name="databasesService">The service for interacting with databases.</param>
    /// <param name="logger">The logger instance for logging information, warnings, and errors.</param>
    public DatabasesController(IDatabasesService databasesService, ILogger<DatabasesController> logger)
    {
        _databasesService = databasesService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a list of databases matching the provided query parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering the database results.</param>
    /// <returns>An IActionResult containing a list of matching databases.</returns>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Get([FromQuery] DatabaseBrowserQuery query)
    {
        var response = await _databasesService.GetAsync(query, CancellationToken.None);
        return new OkObjectResult(response);
    }

    /// <summary>
    /// Gets the database with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the database to retrieve.</param>
    /// <returns>An IActionResult containing the database with the specified ID.</returns>
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await _databasesService.GetAsync(id, CancellationToken.None);
        return new OkObjectResult(response);
    }

    /// <summary>
    /// Creates a new database with the specified properties.
    /// </summary>
    /// <param name="body">The properties of the new database to create.</param>
    /// <returns>An IActionResult containing the newly created database.</returns>
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Create([FromBody] PostDatabaseRequest body)
    {
        var responseMessage = await _databasesService.CreateAsync(body, CancellationToken.None);

        // Create the location URI for the new resource
        var newResourceUrl = Url.Action("Get", new { id = responseMessage.Response.Result })!;
        var locationUri = new Uri(newResourceUrl, UriKind.Relative);

        // Return the CreatedResult with the location URI
        return new CreatedResult(locationUri, responseMessage);
    }

    /// <summary>
    /// Updates the database with the specified ID with the new properties provided.
    /// </summary>
    /// <param name="id">The ID of the database to update.</param>
    /// <param name="body">The new properties of the database to update.</param>
    /// <returns>An IActionResult containing the updated database.</returns>
    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] UpdateDatabaseRequest body)
    {
        var response = await _databasesService.UpdateAsync(id, body, CancellationToken.None);
        return new OkObjectResult(response);
    }

    /// <summary>
    /// Deletes the database with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the database to delete.</param>
    /// <returns>An IActionResult indicating the success or failure of the delete operation.</returns>
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _databasesService.DeleteAsync(id, CancellationToken.None);
        return new OkObjectResult(response);
    }
}