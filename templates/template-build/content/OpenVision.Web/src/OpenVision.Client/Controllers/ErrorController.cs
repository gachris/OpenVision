using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OpenVision.Client.Controllers;

/// <summary>
/// Handles error responses and displays friendly error pages.
/// </summary>
public class ErrorController : Controller
{
    #region Fields/Consts

    private readonly ILogger<ErrorController> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorController"/> class.
    /// </summary>
    /// <param name="logger">The logger for capturing error details.</param>
    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Handles unhandled exceptions and displays a friendly error view.
    /// </summary>
    /// <returns>
    /// An <see cref="IActionResult"/> that renders the "Error" view with error details.
    /// </returns>
    /// <remarks>
    /// This action is executed when an unhandled exception occurs. It retrieves the exception details
    /// from the current HTTP context and logs the error, including the request identifier and the path
    /// where the error occurred.
    /// </remarks>
    [Route("error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionFeature != null)
        {
            _logger.LogError(exceptionFeature.Error,
                "Unhandled exception occurred. Request ID: {RequestId}. Request Path: {Path}",
                requestId, exceptionFeature.Path);
        }
        else
        {
            // Fallback logging if no exception details are found.
            _logger.LogError("An error occurred but no exception details were captured. Request ID: {RequestId}",
                requestId);
        }

        ViewBag.ExceptionPath = exceptionFeature?.Path;
        ViewBag.ExceptionMessage = exceptionFeature?.Error?.Message;

        return View("Error");
    }

    /// <summary>
    /// Handles HTTP status code errors, such as 404 Not Found, and displays a friendly error view.
    /// </summary>
    /// <param name="code">The HTTP status code that triggered the error.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> that renders the "Error" view with a corresponding error message.
    /// </returns>
    /// <remarks>
    /// This action is executed for HTTP status code errors (for example, 404 errors) using the status code
    /// pages middleware. The action logs the error and displays a customized message depending on the status code.
    /// </remarks>
    [Route("error/{code}")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult HttpStatusCodeHandler(int code)
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

        _logger.LogError("HTTP status code error occurred. Request ID: {RequestId}, Status Code: {StatusCode}",
            requestId, code);

        ViewBag.StatusCode = code;
        ViewBag.ErrorMessage = code switch
        {
            404 => "Sorry, the resource you are looking for could not be found.",
            _ => "An unexpected error occurred.",
        };

        return View("Error");
    }

    #endregion
}
