using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenVision.Client.Core.Controllers;
using OpenVision.Client.Core.ViewModels;

namespace OpenVision.Client.Controllers;

/// <summary>
/// Controller for the home page and error pages.
/// </summary>
public class HomeController : BaseController
{
    #region Fields/Consts

    private readonly ILogger<HomeController> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the HomeController class.
    /// </summary>
    /// <param name="logger">The logger for the controller.</param>
    public HomeController(ILogger<HomeController> logger) : base(logger)
    {
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Displays the home page.
    /// </summary>
    public IActionResult Index()
    {
        _logger.LogInformation("Home page requested.");
        return View();
    }

    /// <summary>
    /// Displays the support page.
    /// </summary>
    public IActionResult Support()
    {
        _logger.LogInformation("Support page requested.");
        return View();
    }

    /// <summary>
    /// Displays the error page for unhandled exceptions.
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogError("Error page requested. Request ID: {RequestId}", requestId);

        var error = new ErrorViewModel
        {
            RequestId = requestId
        };

        return View(error);
    }

    #endregion
}
