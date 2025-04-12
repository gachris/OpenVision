using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpsPolicy;

namespace OpenVision.Client.Core.Configuration;

/// <summary>
/// Represents the security configuration settings for the OpenVision Client application.
/// </summary>
public class SecurityConfiguration
{
    /// <summary>
    /// Gets or sets the list of trusted domains that are allowed for Content Security Policy (CSP).
    /// </summary>
    public List<string> CspTrustedDomains { get; set; } = new List<string>();

    /// <summary>
    /// Gets or sets a value indicating whether the Developer Exception Page should be enabled.
    /// </summary>
    public bool UseDeveloperExceptionPage { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether HTTP Strict Transport Security (HSTS) is enabled.
    /// </summary>
    public bool UseHsts { get; set; } = true;

    /// <summary>
    /// Gets or sets an action to configure HSTS options.
    /// </summary>
    public Action<HstsOptions>? HstsConfigureAction { get; set; }

    /// <summary>
    /// Gets or sets an action to configure authentication services using an <see cref="AuthenticationBuilder"/>.
    /// </summary>
    public Action<AuthenticationBuilder>? AuthenticationBuilderAction { get; set; }

    /// <summary>
    /// Gets or sets an action to configure authorization options.
    /// </summary>
    public Action<AuthorizationOptions>? AuthorizationConfigureAction { get; set; }
}
