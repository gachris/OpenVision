using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Contracts;
using OpenVision.Shared.Requests;

namespace OpenVision.Server.Core.Controllers;

/// <summary>
/// API controller for handling web server-related operations.
/// </summary>
[ApiController]
[Route("api/ws")]
[Authorize(Policy = AuthorizationConsts.ServerApiKeyPolicy)]
public class WebServerController : ControllerBase
{
    #region Fields/Consts

    /// <summary>
    /// The service for interacting with targets.
    /// </summary>
    private readonly IWebServerService _webServerService;

    /// <summary>
    /// The logger instance for logging information, warnings, and errors.
    /// </summary>
    private readonly ILogger<WebServerController> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="WebServerController"/> class.
    /// </summary>
    /// <param name="webServerService">Service for handling web server operations.</param>
    /// <param name="logger">Logger instance for logging.</param>
    public WebServerController(IWebServerService webServerService, ILogger<WebServerController> logger)
    {
        _webServerService = webServerService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all web server resources.
    /// </summary>
    /// <returns>A list of all web server resources.</returns>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Get()
    {
        var response = await _webServerService.GetAsync(CancellationToken.None);
        return new OkObjectResult(response);
    }

    /// <summary>
    /// Gets a specific web server resource by ID.
    /// </summary>
    /// <param name="id">The ID of the web server resource.</param>
    /// <returns>The requested web server resource.</returns>
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var response = await _webServerService.GetAsync(id, CancellationToken.None);
        return new OkObjectResult(response);
    }

    /// <summary>
    /// Creates a new web server resource.
    /// </summary>
    /// <param name="body">The request body containing the details of the resource to be created.</param>
    /// <returns>The created web server resource.</returns>
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Create([FromBody] PostTrackableRequest body)
    {
        var responseMessage = await _webServerService.CreateAsync(body, CancellationToken.None);

        // Create the location URI for the new resource
        var uriString = Url.Action("Get", new { id = responseMessage.Response.Result });

        var locationUri = !string.IsNullOrEmpty(uriString) ? new Uri(uriString, UriKind.Relative) : null;

        return new CreatedResult(locationUri, responseMessage);
    }

    /// <summary>
    /// Updates an existing web server resource.
    /// </summary>
    /// <param name="id">The ID of the web server resource to update.</param>
    /// <param name="body">The request body containing the updated details of the resource.</param>
    /// <returns>The updated web server resource.</returns>
    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] UpdateTrackableRequest body)
    {
        var response = await _webServerService.UpdateAsync(id, body, CancellationToken.None);
        return new OkObjectResult(response);
    }

    /// <summary>
    /// Deletes a specific web server resource by ID.
    /// </summary>
    /// <param name="id">The ID of the web server resource to delete.</param>
    /// <returns>The result of the delete operation.</returns>
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await _webServerService.DeleteAsync(id, CancellationToken.None);
        return new OkObjectResult(response);
    }
}