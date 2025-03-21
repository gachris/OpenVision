using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Services;
using OpenVision.Shared.Requests;
using OpenVision.Web.Core.Filters;

namespace OpenVision.Server.Core.Controllers;

/// <summary>
/// API controller for managing targets.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationConsts.BearerPolicy)]
public class TargetsController : ControllerBase
{
    #region Fields/Consts

    /// <summary>
    /// The service for interacting with targets.
    /// </summary>
    private readonly ITargetsService _targetsService;

    /// <summary>
    /// The logger instance for logging information, warnings, and errors.
    /// </summary>
    private readonly ILogger<TargetsController> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetsController"/> class.
    /// </summary>
    /// <param name="targetsService">The service for interacting with targets.</param>
    /// <param name="logger">The logger instance for logging information, warnings, and errors.</param>
    public TargetsController(ITargetsService targetsService, ILogger<TargetsController> logger)
    {
        _targetsService = targetsService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a list of targets matching the provided query parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering the target results.</param>
    /// <returns>An IActionResult containing a list of matching targets.</returns>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Get([FromQuery] TargetBrowserQuery query)
    {
        var response = await _targetsService.GetAsync(query, CancellationToken.None);
        return new OkObjectResult(response);
    }

    /// <summary>
    /// Gets the target with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the target to retrieve.</param>
    /// <returns>An IActionResult containing the target with the specified ID.</returns>
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await _targetsService.GetAsync(id, CancellationToken.None);
        return new OkObjectResult(response);
    }

    /// <summary>
    /// Creates a new target with the specified properties.
    /// </summary>
    /// <param name="body">The properties of the new target to create.</param>
    /// <returns>An IActionResult containing the newly created target.</returns>
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Create([FromBody] PostTargetRequest body)
    {
        var responseMessage = await _targetsService.CreateAsync(body, CancellationToken.None);

        // Create the location URI for the new resource
        var newResourceUrl = Url.Action("Get", new { id = responseMessage.Response.Result })!;
        var locationUri = new Uri(newResourceUrl, UriKind.Relative);

        // Return the CreatedResult with the location URI
        return new CreatedResult(locationUri, responseMessage);
    }

    /// <summary>
    /// Updates the target with the specified ID with the new properties provided.
    /// </summary>
    /// <param name="id">The ID of the target to update.</param>
    /// <param name="body">The new properties of the target to update.</param>
    /// <returns>An IActionResult containing the updated target.</returns>
    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] UpdateTargetRequest body)
    {
        var response = await _targetsService.UpdateAsync(id, body, CancellationToken.None);
        return new OkObjectResult(response);
    }

    /// <summary>
    /// Deletes the target with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the target to delete.</param>
    /// <returns>An IActionResult indicating the success or failure of the delete operation.</returns>
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _targetsService.DeleteAsync(id, CancellationToken.None);
        return new OkObjectResult(response);
    }
}
