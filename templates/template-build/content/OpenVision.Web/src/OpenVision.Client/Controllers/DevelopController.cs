using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenVision.Client.Core.Controllers;
using OpenVision.Client.Core.Requests;
using OpenVision.Client.Core.Services;
using OpenVision.Client.Core.ViewModels;
using OpenVision.Shared;
using OpenVision.Shared.Requests;
using ResponseStatusCode = OpenVision.Shared.StatusCode;

namespace OpenVision.Client.Controllers;

/// <summary>
/// Controller for database development actions.
/// </summary>
[Authorize]
public class DevelopController : BaseController
{
    #region Fields/Consts

    private readonly IDatabasesService _databasesService;
    private readonly IFilesService _filesService;
    private readonly ITargetsService _targetsService;
    private readonly ILogger<DevelopController> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the DevelopController class.
    /// </summary>
    /// <param name="databasesService">The service for database-related actions.</param>
    /// <param name="filesService">The service for file-related actions.</param>
    /// <param name="targetsService">The service for target-related actions.</param>
    /// <param name="logger">The logger for the controller.</param>
    public DevelopController(
        IDatabasesService databasesService,
        IFilesService filesService,
        ITargetsService targetsService,
        ILogger<DevelopController> logger) : base(logger)
    {
        _databasesService = databasesService;
        _filesService = filesService;
        _targetsService = targetsService;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Displays a paginated list of all databases.
    /// </summary>
    /// <param name="page">The page number to display.</param>
    /// <param name="search">The search text to filter items.</param>
    [HttpGet]
    public async Task<IActionResult> Databases(int? page, string search)
    {
        ViewBag.Search = search;

        var databaseBrowserQuery = new DatabaseBrowserQuery() { Page = page ?? 1, Size = 9, Description = search };
        var responseMessage = await _databasesService.GetAsync(databaseBrowserQuery);

        if (responseMessage.StatusCode == ResponseStatusCode.Success)
        {
            return View(responseMessage);
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
            return RedirectToAction(nameof(HomeController.Index), "home");
        }
    }

    /// <summary>
    /// Displays a database details by id.
    /// </summary>
    /// <param name="id">The id of the database to display.</param>
    [HttpGet]
    public async Task<IActionResult> Database(Guid id)
    {
        var responseMessage = await _databasesService.GetAsync(id);

        if (responseMessage.StatusCode == ResponseStatusCode.Success)
        {
            return View(responseMessage.Response.Result);
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
            return RedirectToAction(nameof(Databases));
        }
    }

    /// <summary>
    /// Creates a new database and redirects to Databases action.
    /// </summary>
    /// <param name="viewModel">The request object containing the new database information.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDatabase(PostDatabaseViewModel viewModel)
    {
        var postDatabaseRequest = new PostDatabaseRequest()
        {
            Name = viewModel.Name,
            Type = viewModel.Type
        };

        var responseMessage = await _databasesService.CreateAsync(postDatabaseRequest);

        if (responseMessage.StatusCode == ResponseStatusCode.Success)
        {
            SuccessNotification($"Database is successfully created!", "Success");
            return RedirectToAction(nameof(Database), new { id = responseMessage.Response.Result.Id });
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
            return RedirectToAction(nameof(Databases));
        }
    }

    /// <summary>
    /// Updates the name of a database by id and redirects to Database action.
    /// </summary>
    /// <param name="viewModel">The request object containing the database information.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateDatabaseName(UpdateTargetNameViewModel viewModel)
    {
        var databaseId = viewModel.Id!.Value;
        var responseMessage = await _databasesService.UpdateAsync(databaseId, new UpdateDatabaseRequest() { Name = viewModel.Name });

        if (responseMessage.StatusCode == ResponseStatusCode.Success)
        {
            SuccessNotification($"Database is successfully updated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
        }

        return RedirectToAction(nameof(Database), new { id = databaseId });
    }

    /// <summary>
    /// Downloads a database by id.
    /// </summary>
    /// <param name="id">The id of the database to download.</param>
    [HttpGet]
    public async Task<IActionResult> DownloadDatabase(Guid id)
    {
        var responseMessage = await _filesService.DownloadAsync(id);

        if (responseMessage.StatusCode == ResponseStatusCode.Failed)
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
            return RedirectToAction(nameof(Target), new { id });
        }

        return File(responseMessage.Response.Result.FileContents, responseMessage.Response.Result.ContentType, responseMessage.Response.Result.Filename);
    }

    /// <summary>
    /// Deletes a database by id and redirects to Databases action.
    /// </summary>
    /// <param name="viewModel">The request object containing the database information.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDatabase(DeleteDatabaseViewModel viewModel)
    {
        var databaseId = viewModel.Id!.Value;
        var responseMessage = await _databasesService.DeleteAsync(databaseId);

        if (responseMessage.StatusCode is ResponseStatusCode.Success)
        {
            SuccessNotification($"Database is successfully deleted!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
        }

        return Redirect(nameof(Databases));
    }

    /// <summary>
    /// Displays a target details by id.
    /// </summary>
    /// <param name="id">The id of the target to display.</param>
    [HttpGet]
    public async Task<IActionResult> Target(Guid id)
    {
        var responseMessage = await _targetsService.GetAsync(id);

        if (responseMessage.StatusCode == ResponseStatusCode.Success)
        {
            return View(responseMessage.Response.Result);
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
            return RedirectToAction(nameof(Databases));
        }
    }

    /// <summary>
    /// Creates a new target for a database and redirects to Database action.
    /// </summary>
    /// <param name="viewModel">The request object containing the new target information.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateTarget(PostTargetViewModel viewModel)
    {
        var imageFile = viewModel.Image!;
        var databaseId = viewModel.DatabaseId!.Value;

        if (!(imageFile.ContentType.Equals("image/jpg") || imageFile.ContentType.Equals("image/jpeg") || imageFile.ContentType.Equals("image/png")))
        {
            ErrorNotification($"The image must be jpg or png.", "Validation error");
            return RedirectToAction(nameof(Database), new { id = databaseId });
        }

        if (imageFile.Length > 2097152)
        {
            ErrorNotification($"Image must be less than or equal to 2 MB.", "Validation error");
            return RedirectToAction(nameof(Database), new { id = databaseId });
        }

        using var ms = new MemoryStream();
        imageFile.CopyTo(ms);

        var postTargetRequest = new PostTargetRequest()
        {
            Name = viewModel.Name,
            Type = viewModel.Type,
            Image = ms.ToArray(),
            DatabaseId = databaseId,
            Width = viewModel.Width,
            ActiveFlag = ActiveFlag.True,
        };

        if (viewModel.Metadata is not null)
        {
            using var streamReader = new StreamReader(viewModel.Metadata.OpenReadStream());

            if (viewModel.Metadata.Length > 1048576)
            {
                ErrorNotification($"Metadata must be less than or equal to 1 MB.", "Validation error");
                return RedirectToAction(nameof(Database), new { id = databaseId });
            }

            postTargetRequest.Metadata = await streamReader.ReadToEndAsync();
        }

        var responseMessage = await _targetsService.CreateAsync(postTargetRequest);

        if (responseMessage.StatusCode == ResponseStatusCode.Success)
        {
            SuccessNotification($"Target is successfully created!", "Success");
            return RedirectToAction(nameof(Target), new { id = responseMessage.Response.Result.Id });
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
            return RedirectToAction(nameof(Database), new { id = databaseId });
        }
    }

    /// <summary>
    /// Updates the name of a target by id and redirects to Target action.
    /// </summary>
    /// <param name="viewModel">The request object containing the target information.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTargetName(UpdateTargetNameViewModel viewModel)
    {
        var id = viewModel.Id!.Value;
        var updateTargetRequest = new UpdateTargetRequest { Name = viewModel.Name };
        var responseMessage = await _targetsService.UpdateAsync(id, updateTargetRequest);

        if (responseMessage.StatusCode == ResponseStatusCode.Success)
        {
            SuccessNotification($"Target is successfully updated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
        }

        return RedirectToAction(nameof(Target), new { id });
    }

    /// <summary>
    /// Updates the width of a target by id and redirects to Target action.
    /// </summary>
    /// <param name="viewModel">The request object containing the target information.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTargetWidth(UpdateTargetWidthViewModel viewModel)
    {
        var targetId = viewModel.Id!.Value;

        var responseMessage = await _targetsService.UpdateAsync(targetId, new UpdateTargetRequest { Width = viewModel.Width });

        if (responseMessage.StatusCode == ResponseStatusCode.Success)
        {
            SuccessNotification($"Target is successfully updated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
        }

        return RedirectToAction(nameof(Target), new { id = targetId });
    }

    /// <summary>
    /// Updates an image of a target by id and redirects to Target action.
    /// </summary>
    /// <param name="viewModel">The request object containing the target information.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateTargetImage(UpdateTargetImageViewModel viewModel)
    {
        var targetId = viewModel.Id!.Value;
        var imageFile = viewModel.Image!;

        if (!(imageFile.ContentType.Equals("image/jpg") || imageFile.ContentType.Equals("image/jpeg") || imageFile.ContentType.Equals("image/png")))
        {
            ErrorNotification($"The image must be jpg or png.", "Validation error");
            return RedirectToAction(nameof(Target), new { id = targetId });
        }

        var maxImageSize = 2 * 1024 * 1024;

        if (imageFile.Length > maxImageSize)
        {
            ErrorNotification($"Image must be less than or equal to {maxImageSize / (1024 * 1024)} MB.", "Validation error");
            return RedirectToAction(nameof(Target), new { id = targetId });
        }

        using var ms = new MemoryStream();
        imageFile.CopyTo(ms);

        var updateTargetRequest = new UpdateTargetRequest { Image = ms.ToArray() };
        var responseMessage = await _targetsService.UpdateAsync(targetId, updateTargetRequest);

        if (responseMessage.StatusCode == ResponseStatusCode.Success)
        {
            SuccessNotification($"Target is successfully updated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
        }

        return RedirectToAction(nameof(Target), new { id = targetId });
    }

    /// <summary>
    /// Uploads a metadata file of a target by id and redirects to Target action.
    /// </summary>
    /// <param name="viewModel">The request object containing the target information.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadTargetMetadata(UploadTargetMetadataViewModel viewModel)
    {
        var targetId = viewModel.Id!.Value;
        var metadataFile = viewModel.Metadata!;
        var maxMetadataSize = 1024 * 1024;

        if (metadataFile.Length > maxMetadataSize)
        {
            ErrorNotification($"Metadata must be less than or equal to {maxMetadataSize / (1024 * 1024)} MB.", "Validation error");
            return RedirectToAction(nameof(Target), new { id = targetId });
        }

        using var streamReader = new StreamReader(metadataFile.OpenReadStream());
        var content = await streamReader.ReadToEndAsync();
        var updateTargetRequest = new UpdateTargetRequest { Metadata = content };
        var responseMessage = await _targetsService.UpdateAsync(targetId, updateTargetRequest);

        if (responseMessage.StatusCode == ResponseStatusCode.Success)
        {
            SuccessNotification($"Target is successfully updated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
        }

        return RedirectToAction(nameof(Target), new { id = targetId });
    }

    /// <summary>
    /// Downloads the metadata file of a target by id.
    /// </summary>
    /// <param name="id">The id of the target to update.</param>
    [HttpGet]
    public async Task<IActionResult> DownloadTargetMetadata(Guid id)
    {
        var responseMessage = await _targetsService.GetAsync(id);

        if (responseMessage.StatusCode == ResponseStatusCode.Failed)
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
            return RedirectToAction(nameof(Target), new { id });
        }

        // Get the metadata for the file based on the content
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(responseMessage.Response.Result.Metadata));

        var metadata = memoryStream.ToArray();

        var filename = Path.ChangeExtension(responseMessage.Response.Result.Name, ".metadata");

        return File(metadata, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
    }

    /// <summary>
    /// Deletes the metadata file of a target by id and redirects to Target action.
    /// </summary>
    /// <param name="id">The id of the target to update.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTargetMetadata(Guid id)
    {
        var responseMessage = await _targetsService.UpdateAsync(id, new UpdateTargetRequest { Metadata = string.Empty });

        if (responseMessage.StatusCode == ResponseStatusCode.Success)
        {
            SuccessNotification($"Target is successfully updated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
        }

        return RedirectToAction(nameof(Target), new { id });
    }

    /// <summary>
    /// Activates a target by id and redirects to Target action.
    /// </summary>
    /// <param name="id">The id of the target to deactivate.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(Guid id)
    {
        var responseMessage = await _targetsService.UpdateAsync(id, new UpdateTargetRequest { ActiveFlag = ActiveFlag.True });

        if (responseMessage.StatusCode == ResponseStatusCode.Success)
        {
            SuccessNotification($"Target is successfully updated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
        }

        return RedirectToAction(nameof(Target), new { id });
    }

    /// <summary>
    /// Deactivates a target by id and redirects to Target action.
    /// </summary>
    /// <param name="id">The id of the target to deactivate.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var responseMessage = await _targetsService.UpdateAsync(id, new UpdateTargetRequest { ActiveFlag = ActiveFlag.False });

        if (responseMessage.StatusCode == ResponseStatusCode.Success)
        {
            SuccessNotification($"Target is successfully updated!", "Success");
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
        }

        return RedirectToAction(nameof(Target), new { id });
    }

    /// <summary>
    /// Deletes a target by id and redirects to Database action.
    /// </summary>
    /// <param name="viewModel">The request object containing the target information.</param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteTarget(DeleteTargetViewModel viewModel)
    {
        var targetId = viewModel.TargetId!.Value;
        var databaseId = viewModel.DatabaseId!.Value;

        var responseMessage = await _targetsService.DeleteAsync(targetId);

        if (responseMessage.StatusCode == ResponseStatusCode.Success)
        {
            SuccessNotification($"Target is successfully updated!", "Success");
            return RedirectToAction(nameof(Database), new { id = databaseId });
        }
        else
        {
            ErrorNotification($"An error occurred: {responseMessage.Errors.First().Message}", "Error");
            return RedirectToAction(nameof(Target), new { id = targetId });
        }
    }

    #endregion
}
