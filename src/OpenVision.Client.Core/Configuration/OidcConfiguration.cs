namespace OpenVision.Client.Core.Configuration;

/// <summary>
/// Represents the application configuration settings.
/// </summary>
public class OidcConfiguration
{
    /// <summary>
    /// Gets or sets the base URL of the IdentityServer.
    /// </summary>
    public required string Authority { get; set; }

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
    public required string ResponseType { get; set; }

    /// <summary>
    /// Gets or sets the URI to redirect to after signing in or out of IdentityAdmin.
    /// </summary>
    public required string RedirectUri { get; set; }

    /// <summary>
    /// Gets or sets the name of the identity cookie.
    /// </summary>
    public required string CookieName { get; set; }

    /// <summary>
    /// Gets or sets the number of hours until the IdentityAdmin cookie expires.
    /// </summary>
    public required double CookieExpiresUtcHours { get; set; }

    /// <summary>
    /// Gets or sets the name of the token validation claim.
    /// </summary>
    public required string TokenValidationClaimName { get; set; }

    /// <summary>
    /// Gets or sets the role of the token validation claim.
    /// </summary>
    public required string TokenValidationClaimRole { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether HTTPS metadata is required for authorization.
    /// </summary>
    public required bool RequireHttpsMetadata { get; set; }

    /// <summary>
    /// Gets or sets the list of scopes required for authorization.
    /// </summary>
    public required string[] Scopes { get; set; }
}