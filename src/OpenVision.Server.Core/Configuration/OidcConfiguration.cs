namespace OpenVision.Server.Core.Configuration;

/// <summary>
/// Provides configuration options for OpenID Connect (OIDC) authentication.
/// </summary>
public class OidcConfiguration
{
    /// <summary>
    /// Gets or sets the authority URL for the OIDC provider.
    /// </summary>
    public virtual string Authority { get; set; } = null!;

    /// <summary>
    /// Gets or sets a value indicating whether HTTPS metadata is required.
    /// </summary>
    public virtual bool RequireHttpsMetadata { get; set; }

    /// <summary>
    /// Gets or sets the expected audience for the access tokens.
    /// </summary>
    public virtual string Audience { get; set; } = null!;

    /// <summary>
    /// Gets or sets the scopes required for accessing the API.
    /// </summary>
    public virtual string[] Scopes { get; set; } = null!;
}