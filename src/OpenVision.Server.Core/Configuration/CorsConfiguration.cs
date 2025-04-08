namespace OpenVision.Server.Core.Configuration;

/// <summary>
/// Represents the API configuration settings for Cross-Origin Resource Sharing (CORS).
/// </summary>
public class CorsConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether CORS requests from any origin are allowed.
    /// </summary>
    public bool CorsAllowAnyOrigin { get; set; }

    /// <summary>
    /// Gets or sets the list of allowed origins for CORS.
    /// </summary>
    public string[] CorsAllowOrigins { get; set; } = [];
}
