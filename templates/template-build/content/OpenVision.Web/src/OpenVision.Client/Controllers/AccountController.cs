using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenVision.Client.Core.Constants;
using OpenVision.Client.Core.Controllers;

namespace OpenVision.Client.Controllers;

/// <summary>
/// Controller for handling user account related actions such as login and logout.
/// </summary>
public class AccountController : BaseController
{
    #region Fields/Consts

    private readonly ILogger<AccountController> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountController"/> class.
    /// </summary>
    /// <param name="logger">Logger instance for logging account controller related activities.</param>
    public AccountController(ILogger<AccountController> logger) : base(logger)
    {
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Action method to initiate user login.
    /// </summary>
    /// <returns>Returns a challenge response to the configured authentication provider.</returns>
    public IActionResult Login()
    {
        _logger.LogInformation("Login initiated.");
        var authenticationProperties = new AuthenticationProperties
        {
            RedirectUri = "/"
        };

        _logger.LogInformation("Redirecting to authentication provider.");
        return Challenge(authenticationProperties);
    }

    /// <summary>
    /// Action method to initiate user logout.
    /// </summary>
    /// <returns>Returns a sign out result for the configured authentication and OpenID Connect schemes.</returns>
    [Authorize]
    public IActionResult Logout()
    {
        _logger.LogInformation("Logout initiated.");
        var authenticationSchemes = new List<string>
        {
            AuthenticationConsts.SignInScheme,
            AuthenticationConsts.OidcAuthenticationScheme
        };

        _logger.LogInformation("Signing out of authentication schemes.");
        return new SignOutResult(authenticationSchemes);
    }

    #endregion
}