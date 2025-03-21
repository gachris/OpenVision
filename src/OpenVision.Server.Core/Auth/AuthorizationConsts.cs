namespace OpenVision.Server.Core.Auth;

/// <summary>
/// Constants for authorization policies used in the application.
/// </summary>
public class AuthorizationConsts
{
    /// <summary>
    /// Policy name requiring administrator role.
    /// </summary>
    public const string BearerPolicy = "RequireBearer";

    /// <summary>
    /// Policy name requiring client API key.
    /// </summary>
    public const string ClientApiKeyPolicy = "RequireClientApiKey";

    /// <summary>
    /// Policy name requiring server API key.
    /// </summary>
    public const string ServerApiKeyPolicy = "RequireServerApiKey";
}
