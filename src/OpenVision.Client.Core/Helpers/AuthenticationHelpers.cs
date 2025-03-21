using Microsoft.AspNetCore.Http;

namespace OpenVision.Client.Core.Helpers;

/// <summary>
/// Provides helper methods for working with authentication-related operations.
/// </summary>
public static class AuthenticationHelpers
{
    /// <summary>
    /// Checks if the SameSite cookie option is set to None and changes it to Unspecified if necessary based on the user agent and the request protocol.
    /// </summary>
    /// <param name="httpContext">The current HttpContext instance.</param>
    /// <param name="options">The CookieOptions instance to modify if necessary.</param>
    public static void CheckSameSite(HttpContext httpContext, CookieOptions options)
    {
        if (options.SameSite == SameSiteMode.None)
        {
            string userAgent = httpContext.Request.Headers["User-Agent"].ToString();
            if (!httpContext.Request.IsHttps || DisallowsSameSiteNone(userAgent))
            {
                options.SameSite = SameSiteMode.Unspecified;
            }
        }
    }

    /// <summary>
    /// Checks if the user agent disallows the SameSite cookie option to be set to None.
    /// </summary>
    /// <param name="userAgent">The User-Agent header value of the current request.</param>
    /// <returns>True if the SameSite cookie option must be set to Unspecified; otherwise, false.</returns>
    public static bool DisallowsSameSiteNone(string userAgent)
    {
        return userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12")
            || userAgent.Contains("Macintosh; Intel Mac OS X 10_14") && userAgent.Contains("Version/") && userAgent.Contains("Safari")
            || userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6");
    }
}