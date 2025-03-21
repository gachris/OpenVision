namespace OpenVision.Server.Core.Auth;

/// <summary>
/// Default values related to API key authentication.
/// </summary>
public class ApiKeyDefaults
{
    /// <summary>
    /// Header key for API key in HTTP requests.
    /// </summary>
    public const string X_API_KEY = "X-API-KEY";

    /// <summary>
    /// Default authentication scheme name for API key authentication.
    /// </summary>
    public const string AuthenticationScheme = "ApiKey";
}
