using System.Text;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenVision.Client.Core.Controllers;
using OpenVision.Client.Core.Mediator.Commands;
using OpenVision.Client.Core.Mediator.Queries;
using OpenVision.Client.Core.ViewModels;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Types;

namespace OpenVision.Client.Controllers;

/// <summary>
/// Controller for target development actions using MediatR commands and queries.
/// </summary>
[Authorize]
[Route("databases/{databaseId:guid}/targets")]
public class TargetsController : BaseController
{
    #region Fields

    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<TargetsController> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetsController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator used for sending commands and queries.</param>
    /// <param name="mapper">The AutoMapper instance for mapping view models to requests.</param>
    /// <param name="logger">The logger instance.</param>
    public TargetsController(
        IMediator mediator,
        IMapper mapper,
        ILogger<TargetsController> logger)
        : base(logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    #region Actions

    /// <summary>
    /// Retrieves the details of a specific target.
    /// </summary>
    /// <param name="databaseId">The parent database identifier.</param>
    /// <param name="targetId">The unique identifier of the target.</param>
    /// <returns>A view displaying the target details, or an error notification if retrieval fails.</returns>
    [HttpGet("{targetId:guid}")]
    public async Task<IActionResult> Details(Guid databaseId, Guid targetId)
    {
        var query = new GetTargetByIdQuery(targetId);
        var result = await _mediator.Send(query);
        if (result.IsSuccess)
        {
            return View(result.Data);
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
            return RedirectToAction("Details", "Databases", new { id = databaseId });
        }
    }

    /// <summary>
    /// Creates a new target for the specified database.
    /// </summary>
    /// <param name="databaseId">The parent database identifier.</param>
    /// <param name="viewModel">The view model containing target creation data.</param>
    /// <returns>A redirection to the details view for the created target if successful; otherwise, returns an error notification.</returns>
    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid databaseId, PostTargetViewModel viewModel)
    {
        // Ensure the database context is set.
        viewModel.DatabaseId = databaseId;

        // Validate image file
        var imageFile = viewModel.Image;
        if (imageFile is null || !(imageFile.ContentType.Equals("image/jpg") ||
            imageFile.ContentType.Equals("image/jpeg") ||
            imageFile.ContentType.Equals("image/png")))
        {
            ErrorNotification("The image must be jpg or png.", "Validation error");
            return RedirectToAction("Details", "Databases", new { id = databaseId });
        }
        if (imageFile.Length > 2 * 1024 * 1024)
        {
            ErrorNotification("Image must be less than or equal to 2 MB.", "Validation error");
            return RedirectToAction("Details", "Databases", new { id = databaseId });
        }
        if (viewModel.Metadata != null && viewModel.Metadata.Length > 1048576)
        {
            ErrorNotification("Metadata must be less than or equal to 1 MB.", "Validation error");
            return RedirectToAction("Details", "Databases", new { id = databaseId });
        }

        var request = _mapper.Map<PostTargetRequest>(viewModel);
        var command = new CreateTargetCommand(request);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            SuccessNotification("Target is successfully created!", "Success");
            return RedirectToAction(nameof(Details), new { databaseId, targetId = result.Data.Id });
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
            return RedirectToAction("Details", "Databases", new { id = databaseId });
        }
    }

    /// <summary>
    /// Updates the target's name.
    /// </summary>
    /// <param name="databaseId">The parent database identifier.</param>
    /// <param name="targetId">The target's unique identifier.</param>
    /// <param name="viewModel">The view model containing the updated name information.</param>
    /// <returns>A redirection to the target details view.</returns>
    [HttpPost("{targetId:guid}/updateName")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateName(Guid databaseId, Guid targetId, UpdateTargetNameViewModel viewModel)
    {
        var updateRequest = _mapper.Map<UpdateTargetRequest>(viewModel);
        var command = new UpdateTargetCommand(targetId, updateRequest);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            SuccessNotification("Target is successfully updated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
        }
        return RedirectToAction(nameof(Details), new { databaseId, targetId });
    }

    /// <summary>
    /// Updates the target's width.
    /// </summary>
    /// <param name="databaseId">The parent database identifier.</param>
    /// <param name="targetId">The target's unique identifier.</param>
    /// <param name="viewModel">The view model containing the updated width information.</param>
    /// <returns>A redirection to the target details view.</returns>
    [HttpPost("{targetId:guid}/updateWidth")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateWidth(Guid databaseId, Guid targetId, UpdateTargetWidthViewModel viewModel)
    {
        var updateRequest = _mapper.Map<UpdateTargetRequest>(viewModel);
        var command = new UpdateTargetCommand(targetId, updateRequest);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            SuccessNotification("Target is successfully updated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
        }
        return RedirectToAction(nameof(Details), new { databaseId, targetId });
    }

    /// <summary>
    /// Updates the target's image.
    /// </summary>
    /// <param name="databaseId">The parent database identifier.</param>
    /// <param name="targetId">The target's unique identifier.</param>
    /// <param name="viewModel">The view model containing the new image file.</param>
    /// <returns>A redirection to the target details view.</returns>
    [HttpPost("{targetId:guid}/updateImage")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateImage(Guid databaseId, Guid targetId, UpdateTargetImageViewModel viewModel)
    {
        // Validate image file
        var imageFile = viewModel.Image;
        if (imageFile is null || !(imageFile.ContentType.Equals("image/jpg") ||
            imageFile.ContentType.Equals("image/jpeg") ||
            imageFile.ContentType.Equals("image/png")))
        {
            ErrorNotification("The image must be jpg or png.", "Validation error");
            return RedirectToAction(nameof(Details), new { databaseId, targetId });
        }
        if (imageFile.Length > 2 * 1024 * 1024)
        {
            ErrorNotification("Image must be less than or equal to 2 MB.", "Validation error");
            return RedirectToAction(nameof(Details), new { databaseId, targetId });
        }

        var updateRequest = _mapper.Map<UpdateTargetRequest>(viewModel);
        var command = new UpdateTargetCommand(targetId, updateRequest);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            SuccessNotification("Target is successfully updated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
        }
        return RedirectToAction(nameof(Details), new { databaseId, targetId });
    }

    /// <summary>
    /// Uploads metadata for the target.
    /// </summary>
    /// <param name="databaseId">The parent database identifier.</param>
    /// <param name="targetId">The target's unique identifier.</param>
    /// <param name="viewModel">The view model containing the metadata file.</param>
    /// <returns>A redirection to the target details view.</returns>
    [HttpPost("{targetId:guid}/uploadMetadata")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadMetadata(Guid databaseId, Guid targetId, UploadTargetMetadataViewModel viewModel)
    {
        if (viewModel.Metadata != null && viewModel.Metadata.Length > 1024 * 1024)
        {
            ErrorNotification("Metadata must be less than or equal to 1 MB.", "Validation error");
            return RedirectToAction(nameof(Details), new { databaseId, targetId });
        }

        var updateRequest = _mapper.Map<UpdateTargetRequest>(viewModel);
        var command = new UpdateTargetCommand(targetId, updateRequest);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            SuccessNotification("Target is successfully updated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
        }
        return RedirectToAction(nameof(Details), new { databaseId, targetId });
    }

    /// <summary>
    /// Downloads the metadata file associated with the target.
    /// </summary>
    /// <param name="databaseId">The parent database identifier.</param>
    /// <param name="targetId">The target's unique identifier.</param>
    /// <returns>A file result containing the downloaded metadata.</returns>
    [HttpGet("{targetId:guid}/downloadMetadata")]
    public async Task<IActionResult> DownloadMetadata(Guid databaseId, Guid targetId)
    {
        var query = new GetTargetByIdQuery(targetId);
        var result = await _mediator.Send(query);
        if (!result.IsSuccess)
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
            return RedirectToAction(nameof(Details), new { databaseId, targetId });
        }

        var metadataContent = result.Data.Metadata ?? string.Empty;
        var metadataBytes = Encoding.UTF8.GetBytes(metadataContent);
        var filename = Path.ChangeExtension(result.Data.Name, ".metadata");
        return File(metadataBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
    }

    /// <summary>
    /// Deletes the metadata associated with the target.
    /// </summary>
    /// <param name="databaseId">The parent database identifier.</param>
    /// <param name="targetId">The target's unique identifier.</param>
    /// <returns>A redirection to the target details view.</returns>
    [HttpPost("{targetId:guid}/deleteMetadata")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMetadata(Guid databaseId, Guid targetId)
    {
        var updateRequest = new UpdateTargetRequest { Metadata = string.Empty };
        var command = new UpdateTargetCommand(targetId, updateRequest);
        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            SuccessNotification("Target metadata is successfully deleted!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
        }

        return RedirectToAction(nameof(Details), new { databaseId, targetId });
    }

    /// <summary>
    /// Activates the target.
    /// </summary>
    /// <param name="databaseId">The parent database identifier.</param>
    /// <param name="targetId">The target's unique identifier.</param>
    /// <returns>A redirection to the target details view.</returns>
    [HttpPost("{targetId:guid}/activate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid databaseId, Guid targetId)
    {
        var command = new UpdateTargetCommand(targetId, new UpdateTargetRequest { ActiveFlag = ActiveFlag.True });
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            SuccessNotification("Target is successfully activated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
        }
        return RedirectToAction(nameof(Details), new { databaseId, targetId });
    }

    /// <summary>
    /// Deactivates the target.
    /// </summary>
    /// <param name="databaseId">The parent database identifier.</param>
    /// <param name="targetId">The target's unique identifier.</param>
    /// <returns>A redirection to the target details view.</returns>
    [HttpPost("{targetId:guid}/deactivate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid databaseId, Guid targetId)
    {
        var command = new UpdateTargetCommand(targetId, new UpdateTargetRequest { ActiveFlag = ActiveFlag.False });
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            SuccessNotification("Target is successfully deactivated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
        }
        return RedirectToAction(nameof(Details), new { databaseId, targetId });
    }

    /// <summary>
    /// Deletes the target.
    /// </summary>
    /// <param name="databaseId">The parent database identifier.</param>
    /// <param name="targetId">The target's unique identifier.</param>
    /// <returns>A redirection to the database details view.</returns>
    [HttpPost("{targetId:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid databaseId, Guid targetId)
    {
        var command = new DeleteTargetCommand(targetId);
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            SuccessNotification("Target is successfully deleted!", "Success");
            return RedirectToAction("Details", "Databases", new { id = databaseId });
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
            return RedirectToAction(nameof(Details), new { databaseId, targetId });
        }
    }

    #endregion
}
