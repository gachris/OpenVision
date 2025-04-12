using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Server.Core.Controllers;

/// <summary>
/// API controller for managing targets.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthorizationConsts.BearerPolicy)]
public class TargetsController : ApiControllerBase
{
    #region Fields/Consts

    private readonly ITargetsService _targetsService;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetsController"/> class.
    /// </summary>
    /// <param name="targetsService">The target service instance.</param>
    /// <param name="mapper">The mapper instance.</param>
    /// <param name="logger">The logger instance.</param>
    public TargetsController(
        ITargetsService targetsService,
        IMapper mapper,
        ILogger<TargetsController> logger) : base(mapper, logger)
    {
        _targetsService = targetsService;
    }

    #region Methods

    /// <summary>
    /// Retrieves a paginated list of targets based on the specified query.
    /// </summary>
    /// <param name="query">The query parameters for browsing targets.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An <see cref="IActionResult"/> containing the paged response of targets.</returns>
    [HttpGet]
    [Route("")]
    public virtual async Task<IActionResult> Get([FromQuery] TargetBrowserQuery query, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to get targets list with query: {@Query}", query);
            var targetDtosQueryable = await _targetsService.GetQueryableAsync(cancellationToken);
            targetDtosQueryable = targetDtosQueryable
                .Where(x => x.Database!.Id == query.DatabaseId)
                .OrderBy(x => x.Created);

            var pagedResponse = await GetPagedResponseAsync<TargetDto, TargetResponse>(targetDtosQueryable, query, cancellationToken);
            _logger.LogInformation("Returning paged targets list with total records: {TotalRecords}", pagedResponse.TotalRecords);
            return new OkObjectResult(pagedResponse);
        });
    }

    /// <summary>
    /// Retrieves a specific target by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the target.</param>
    /// <returns>An <see cref="IActionResult"/> containing the target details.</returns>
    [HttpGet]
    [Route("{id:guid}")]
    public virtual async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to get target with id: {Id}", id);
            var targetDto = await _targetsService.GetAsync(id, cancellationToken);
            _logger.LogInformation("Returning target details for id: {Id}", id);
            return new OkObjectResult(Success(_mapper.Map<TargetResponse>(targetDto)));
        });
    }

    /// <summary>
    /// Creates a new target.
    /// </summary>
    /// <param name="body">The request body containing target creation details.</param>
    /// <returns>An <see cref="IActionResult"/> with the created target information and location header.</returns>
    [HttpPost]
    [Route("")]
    public virtual async Task<IActionResult> Create([FromBody] PostTargetRequest body, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to create a new target with description: {Description}", body.Name);
            var createTargetDto = _mapper.Map<CreateTargetDto>(body);
            var targetDto = await _targetsService.CreateAsync(createTargetDto, cancellationToken);

            var url = Url.Action("Get", new { id = targetDto.Id });
            ArgumentException.ThrowIfNullOrEmpty(url, nameof(url));

            _logger.LogInformation("Target created with id: {TargetId}. Location: {Url}", targetDto.Id, url);
            var locationUri = new Uri(url, UriKind.Relative);
            return new CreatedResult(locationUri, Success(_mapper.Map<TargetResponse>(targetDto)));
        });
    }

    /// <summary>
    /// Updates an existing target.
    /// </summary>
    /// <param name="id">The unique identifier of the target to update.</param>
    /// <param name="body">The request body containing updated target details.</param>
    /// <returns>An <see cref="IActionResult"/> with the update operation result.</returns>
    [HttpPut]
    [Route("{id:guid}")]
    public virtual async Task<IActionResult> Edit(Guid id, [FromBody] UpdateTargetRequest body, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to edit target with id: {Id}", id);
            var updateTargetDto = _mapper.Map<UpdateTargetDto>(body);
            var targetDto = await _targetsService.UpdateAsync(id, updateTargetDto, cancellationToken);
            _logger.LogInformation("Target with id: {Id} updated successfully.", id);
            return new OkObjectResult(Success(_mapper.Map<TargetResponse>(targetDto)));
        });
    }

    /// <summary>
    /// Deletes a target.
    /// </summary>
    /// <param name="id">The unique identifier of the target to delete.</param>
    /// <returns>An <see cref="IActionResult"/> with the deletion operation result.</returns>
    [HttpDelete]
    [Route("{id:guid}")]
    public virtual async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to delete target with id: {Id}", id);
            var deleted = await _targetsService.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Target with id: {Id} deleted successfully.", id);
            return new OkObjectResult(Success(deleted));
        });
    }

    #endregion
}