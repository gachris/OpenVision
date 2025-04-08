namespace OpenVision.Server.Core.Configuration;

/// <summary>
/// Represents the configuration settings for Swagger and OAuth integration.
/// </summary>
public class SwaggerConfiguration
{
    /// <summary>
    /// Gets or sets the Swagger endpoint URL.
    /// </summary>
    public virtual string SwaggerEndpoint { get; set; } = null!;

    /// <summary>
    /// Gets or sets the name displayed for Swagger.
    /// </summary>
    public virtual string SwaggerName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the OAuth client identifier.
    /// </summary>
    public virtual string OAuthClientId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the OAuth application name.
    /// </summary>
    public virtual string OAuthAppName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the version of the API.
    /// </summary>
    public virtual string Version { get; set; } = null!;

    /// <summary>
    /// Gets or sets the authorization URL for OAuth authentication.
    /// </summary>
    public virtual string AuthorizationUrl { get; set; } = null!;

    /// <summary>
    /// Gets or sets the token URL for OAuth authentication.
    /// </summary>
    public virtual string TokenUrl { get; set; } = null!;

    /// <summary>
    /// Gets or sets the audience for OAuth authentication.
    /// </summary>
    public virtual string Audience { get; set; } = null!;

    /// <summary>
    /// Gets or sets the list of OAuth scopes required for authentication.
    /// </summary>
    public virtual string[] Scopes { get; set; } = null!;
}
