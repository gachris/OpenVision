namespace OpenVision.Client.Core.Configuration;

/// <summary>
/// Represents the application configuration settings.
/// </summary>
public class AppConfiguration
{
    /// <summary>
    /// Gets or sets the title of the page.
    /// </summary>
    public required string PageTitle { get; set; }

    /// <summary>
    /// Gets or sets the URI of the favicon.
    /// </summary>
    public required string FaviconUri { get; set; }

    /// <summary>
    /// Gets or sets the URI to redirect to after signing in or out of IdentityAdmin.
    /// </summary>
    public required string IdentityAdminRedirectUri { get; set; }

    /// <summary>
    /// Gets or sets the URL of the cloud API.
    /// </summary>
    public required string CloudApiUrl { get; set; }

    /// <summary>
    /// Gets or sets the list of scopes required for authorization.
    /// </summary>
    public required string[] Scopes { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether HTTPS metadata is required for authorization.
    /// </summary>
    public bool RequireHttpsMetadata { get; set; }

    /// <summary>
    /// Gets or sets the name of the IdentityAdmin cookie.
    /// </summary>
    public required string IdentityAdminCookieName { get; set; }

    /// <summary>
    /// Gets or sets the number of hours until the IdentityAdmin cookie expires.
    /// </summary>
    public double IdentityAdminCookieExpiresUtcHours { get; set; }

    /// <summary>
    /// Gets or sets the name of the token validation claim.
    /// </summary>
    public required string TokenValidationClaimName { get; set; }

    /// <summary>
    /// Gets or sets the role of the token validation claim.
    /// </summary>
    public required string TokenValidationClaimRole { get; set; }

    /// <summary>
    /// Gets or sets the base URL of the IdentityServer.
    /// </summary>
    public required string IdentityServerBaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the client ID.
    /// </summary>
    public required string ClientId { get; set; }

    /// <summary>
    /// Gets or sets the client secret.
    /// </summary>
    public required string ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the OpenID Connect response type.
    /// </summary>
    public required string OidcResponseType { get; set; }

    /// <summary>
    /// Gets or sets the URL of the custom theme CSS, if any.
    /// </summary>
    public string? CustomThemeCss { get; set; }
}