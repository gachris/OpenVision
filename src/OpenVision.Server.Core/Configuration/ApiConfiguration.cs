namespace OpenVision.Server.Core.Configuration;

/// <summary>
/// Configuration settings for API related parameters.
/// </summary>
public class ApiConfiguration
{
    /// <summary>
    /// Name of the API.
    /// </summary>
    public required string ApiName { get; set; }

    /// <summary>
    /// Version of the API.
    /// </summary>
    public required string ApiVersion { get; set; }

    /// <summary>
    /// Authority for authentication.
    /// </summary>
    public required string Authority { get; set; }

    /// <summary>
    /// Base URL of the API.
    /// </summary>
    public required string ApiBaseUrl { get; set; }

    /// <summary>
    /// Audience for authentication.
    /// </summary>
    public required string Audience { get; set; }

    /// <summary>
    /// Client ID for OIDC Swagger UI.
    /// </summary>
    public required string OidcSwaggerUIClientId { get; set; }

    /// <summary>
    /// Indicates if HTTPS metadata is required.
    /// </summary>
    public bool RequireHttpsMetadata { get; set; }

    /// <summary>
    /// Indicates if any origin is allowed for CORS.
    /// </summary>
    public bool CorsAllowAnyOrigin { get; set; }

    /// <summary>
    /// Allowed origins for CORS.
    /// </summary>
    public required string[] CorsAllowOrigins { get; set; }

    /// <summary>
    /// Scopes for the API.
    /// </summary>
    public required string[] Scopes { get; set; }
}
