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
/// API controller for managing trackable records.
/// </summary>
[ApiController]
[Route("api/ws")]
[Authorize(Policy = AuthorizationConsts.ServerApiKeyPolicy)]
public class TrackablesController : ApiControllerBase
{
    #region Fields/Consts

    private readonly ITrackablesService _trackablesService;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TrackablesController"/> class.
    /// </summary>
    /// <param name="trackablesService">The trackables service instance.</param>
    /// <param name="mapper">The mapper instance.</param>
    /// <param name="logger">The logger instance.</param>
    public TrackablesController(
        ITrackablesService trackablesService,
        IMapper mapper,
        ILogger<TrackablesController> logger) : base(mapper, logger)
    {
        _trackablesService = trackablesService;
    }

    #region Methods

    /// <summary>
    /// Retrieves all trackable records.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the list of trackable records.</returns>
    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to get trackables list");
            var targetRecordDtos = await _trackablesService.GetAsync(cancellationToken);
            _logger.LogInformation("Returning trackables list with total records: {TotalRecords}", targetRecordDtos.Count());
            return new OkObjectResult(new GetAllTrackablesResponse(new(_mapper.Map<List<TargetRecordModel>>(targetRecordDtos)), Guid.NewGuid(), Shared.StatusCode.Success, []));
        });
    }

    /// <summary>
    /// Retrieves a specific trackable record by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the trackable record.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the trackable record details.</returns>
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to get trackable with id: {Id}", id);
            var targetRecordDto = await _trackablesService.GetAsync(id, cancellationToken);
            _logger.LogInformation("Returning trackable details for id: {Id}", id);
            return new OkObjectResult(new TrackableRetrieveResponse(new(_mapper.Map<TargetRecordModel>(targetRecordDto)), Guid.NewGuid(), Shared.StatusCode.Success, []));
        });
    }

    /// <summary>
    /// Creates a new trackable record.
    /// </summary>
    /// <param name="body">The request body containing details for creating the trackable record.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> with the created trackable record and location header.</returns>
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> Create([FromBody] PostTrackableRequest body, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to create a new trackable with name: {Name}", body.Name);
            var postTrackableDto = _mapper.Map<PostTrackableDto>(body);
            var targetRecordDto = await _trackablesService.CreateAsync(postTrackableDto, cancellationToken);

            var url = Url.Action("Get", new { id = targetRecordDto.TargetId });
            ArgumentException.ThrowIfNullOrEmpty(url, nameof(url));

            _logger.LogInformation("Trackable created with id: {TargetId}. Location: {Url}", targetRecordDto.TargetId, url);
            var locationUri = new Uri(url, UriKind.Relative);
            return new CreatedResult(locationUri, new PostTrackableResponse(new ResponseDoc<string>(targetRecordDto.TargetId), Guid.NewGuid(), Shared.StatusCode.Success, []));
        });
    }

    /// <summary>
    /// Updates an existing trackable record.
    /// </summary>
    /// <param name="id">The unique identifier of the trackable record to update.</param>
    /// <param name="body">The request body containing updated details of the trackable record.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> containing the update operation result.</returns>
    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] UpdateTrackableRequest body, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to update trackable with id: {Id}", id);
            var targetRecordDto = await _trackablesService.UpdateAsync(id, _mapper.Map<UpdateTrackableDto>(body), cancellationToken);
            _logger.LogInformation("Trackable with id: {Id} updated successfully.", id);
            return new OkObjectResult(Success());
        });
    }

    /// <summary>
    /// Deletes a trackable record.
    /// </summary>
    /// <param name="id">The unique identifier of the trackable record to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="IActionResult"/> with the result of the delete operation.</returns>
    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () =>
        {
            _logger.LogInformation("Received request to delete trackable with id: {Id}", id);
            var deleted = await _trackablesService.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Trackable with id: {Id} deleted successfully.", id);
            return new OkObjectResult(Success());
        });
    }

    #endregion
}