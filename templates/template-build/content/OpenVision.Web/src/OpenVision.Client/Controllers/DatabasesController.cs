using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenVision.Client.Core.Contracts;
using OpenVision.Client.Core.Controllers;
using OpenVision.Client.Core.Mediator.Commands;
using OpenVision.Client.Core.Mediator.Queries;
using OpenVision.Client.Core.ViewModels;
using OpenVision.Shared.Requests;

namespace OpenVision.Client.Controllers;

/// <summary>
/// Controller for database development actions using MediatR commands and queries.
/// </summary>
[Authorize]
[Route("databases")]
public class DatabasesController : BaseController
{
    #region Fields/Consts

    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IFileApiService _filesService;
    private readonly ILogger<DatabasesController> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabasesController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator used for sending commands and queries.</param>
    /// <param name="mapper">The AutoMapper instance for mapping between view models and requests.</param>
    /// <param name="filesService">The file API service.</param>
    /// <param name="logger">The logger.</param>
    public DatabasesController(
        IMediator mediator,
        IMapper mapper,
        IFileApiService filesService,
        ILogger<DatabasesController> logger)
        : base(logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _filesService = filesService;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Retrieves a paginated list of databases.
    /// </summary>
    /// <param name="page">The page number (optional).</param>
    /// <param name="search">The search string (optional).</param>
    /// <returns>A view displaying the list of databases.</returns>
    [HttpGet("")]
    public async Task<IActionResult> Index(int? page, string search)
    {
        ViewBag.Search = search;
        var query = new GetDatabasesQuery(new DatabaseBrowserQuery
        {
            Page = page ?? 1,
            Size = 9,
            Name = search
        });

        var result = await _mediator.Send(query);
        if (result.IsSuccess)
        {
            return View(result.Data);
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
            return RedirectToAction(nameof(Index), "Home");
        }
    }

    /// <summary>
    /// Retrieves the details of a single database.
    /// </summary>
    /// <param name="id">The unique identifier of the database.</param>
    /// <returns>A view displaying the database details.</returns>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var query = new GetDatabaseByIdQuery(id);
        var result = await _mediator.Send(query);
        if (result.IsSuccess)
        {
            return View(result.Data);
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Creates a new database.
    /// </summary>
    /// <param name="viewModel">The view model containing database creation details.</param>
    /// <returns>A redirection to the details view for the created database.</returns>
    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PostDatabaseViewModel viewModel)
    {
        var createRequest = _mapper.Map<PostDatabaseRequest>(viewModel);
        var command = new CreateDatabaseCommand(createRequest);
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            SuccessNotification("Database is successfully created!", "Success");
            return RedirectToAction(nameof(Details), new { id = result.Data.Id });
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Updates an existing database.
    /// </summary>
    /// <param name="id">The unique identifier of the database to update.</param>
    /// <param name="viewModel">The view model containing the update details.</param>
    /// <returns>A redirection to the details view for the updated database.</returns>
    [HttpPost("{id:guid}/update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Guid id, UpdateDatabaseViewModel viewModel)
    {
        var updateRequest = _mapper.Map<UpdateDatabaseRequest>(viewModel);
        var command = new UpdateDatabaseCommand(id, updateRequest);
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            SuccessNotification("Database is successfully updated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    /// <summary>
    /// Downloads a file associated with a database.
    /// </summary>
    /// <param name="id">The unique identifier of the database.</param>
    /// <returns>A file result containing the downloaded file.</returns>
    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        var fileDto = await _filesService.DownloadAsync(id);
        if (fileDto is null)
        {
            ErrorNotification("An error occurred while downloading the file.", "Error");
            return RedirectToAction(nameof(Details), new { id });
        }
        return File(fileDto.FileContents, fileDto.ContentType, fileDto.Filename);
    }

    /// <summary>
    /// Deletes an existing database.
    /// </summary>
    /// <param name="id">The unique identifier of the database.</param>
    /// <param name="viewModel">The view model for database deletion (may contain confirmation details).</param>
    /// <returns>A redirection to the list view.</returns>
    [HttpPost("{id:guid}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, DeleteDatabaseViewModel viewModel)
    {
        var command = new DeleteDatabaseCommand(id);
        var result = await _mediator.Send(command);
        if (result.IsSuccess)
        {
            SuccessNotification("Database is successfully deleted!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {result.Error}", "Error");
        }
        return RedirectToAction(nameof(Index));
    }

    #endregion
}
