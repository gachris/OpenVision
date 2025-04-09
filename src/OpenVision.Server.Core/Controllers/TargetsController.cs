using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenVision.Server.Core.Auth;
using OpenVision.Server.Core.Contracts;
using OpenVision.Server.Core.Dtos;
using OpenVision.Server.Core.Requests;
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
            var pagedResponse = await GetPagedResponseAsync(query, cancellationToken);
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

    #region Helper Methods

    /// <summary>
    /// Builds a pagination URL using the Url.Action method with the specified pagination filter.
    /// </summary>
    /// <param name="filter">The pagination filter containing the page and size parameters.</param>
    /// <returns>A <see cref="Uri"/> representing the pagination link.</returns>
    private Uri BuildPageUri(BrowserQuery filter)
    {
        var request = HttpContext.Request;
        var url = Url.Action("Get", new { page = filter.Page, size = filter.Size });
        var baseUri = $"{request.Scheme}://{request.Host}{url}";

        if (string.IsNullOrEmpty(baseUri))
        {
            throw new InvalidOperationException("Unable to generate URL for the pagination link.");
        }

        return new Uri(baseUri);
    }

    /// <summary>
    /// Retrieves a paginated response of targets for the query parameters.
    /// </summary>
    /// <param name="query">The pagination and filtering query parameters.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IPagedResponse{T}"/> containing a collection of <see cref="TargetResponse"/> objects.</returns>
    private async Task<IPagedResponse<IEnumerable<TargetResponse>>> GetPagedResponseAsync(
        TargetBrowserQuery query,
        CancellationToken cancellationToken)
    {
        var targetsQueryable = await _targetsService.GetQueryableAsync(cancellationToken);
        var validFilter = new BrowserQuery(query.Page, query.Size);
        var take = validFilter.Size;
        var skip = validFilter.Page - 1;

        var totalRecords = await targetsQueryable.CountAsync(cancellationToken);
        var targetDtos = await targetsQueryable
            .Where(x => string.IsNullOrEmpty(query.Name) || x.Name.Contains(query.Name))
            .Where(x => x.Database.Id == query.DatabaseId)
            .OrderBy(x => x.Created)
            .Skip(skip * take)
            .Take(take)
            .ToListAsync(cancellationToken);

        var totalPages = totalRecords / (double)validFilter.Size;
        var roundedTotalPages = Convert.ToInt32(Math.Ceiling(totalPages));

        var nextPage = validFilter.Page >= 1 && validFilter.Page < roundedTotalPages
            ? BuildPageUri(new BrowserQuery(validFilter.Page + 1, validFilter.Size))
            : null;

        var previousPage = validFilter.Page - 1 >= 1 && validFilter.Page <= roundedTotalPages
            ? BuildPageUri(new BrowserQuery(validFilter.Page - 1, validFilter.Size))
            : null;

        var firstPage = BuildPageUri(new BrowserQuery(1, validFilter.Size));
        var lastPage = BuildPageUri(new BrowserQuery(roundedTotalPages, validFilter.Size));

        return new PagedResponse<IEnumerable<TargetResponse>>(
            validFilter.Page,
            validFilter.Size,
            firstPage,
            lastPage,
            roundedTotalPages,
            totalRecords,
            nextPage,
        previousPage,
            new(_mapper.Map<IEnumerable<TargetResponse>>(targetDtos)),
            Guid.NewGuid(),
            Shared.StatusCode.Success,
            []);
    }

    #endregion
}