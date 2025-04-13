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
    [HttpGet("support")]
    public IActionResult Support()
    {
        _logger.LogInformation("Support page requested.");
        return View();
    }

    /// <summary>
    /// Displays the contact form.
    /// </summary>
    /// <returns>The Contact view.</returns>
    [HttpGet("contact")]
    public IActionResult Contact()
    {
        _logger.LogInformation("Contact page requested.");
        return View();
    }

    /// <summary>
    /// Processes the submitted contact form.
    /// </summary>
    /// <param name="model">The contact form data.</param>
    /// <returns>A redirection back to the contact page with a success or error notification.</returns>
    [HttpPost("contact")]
    [ValidateAntiForgeryToken]
    public IActionResult Contact(ContactViewModel model)
    {
        if (ModelState.IsValid)
        {
            // TODO: Add your processing logic (e.g., send an email, save to database)
            _logger.LogInformation("Contact form submitted by {Name} ({Email}).", model.Name, model.Email);

            // Example: a method on your BaseController to show success notifications.
            SuccessNotification("Your message has been sent successfully!", "Success");

            return RedirectToAction("Contact");
        }
        else
        {
            _logger.LogWarning("Contact form submission failed validation.");
        }

        // If model state is invalid, re-display the form with validation messages.
        return View(model);
    }

    #endregion
}
