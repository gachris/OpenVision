namespace OpenVision.Api.Auth;

/// <summary>
/// Represents an access method for authenticating a user credential.
/// </summary>
public interface IAccessMethod
{
    /// <summary>
    /// Intercepts an HTTP request and adds authentication headers to it.
    /// </summary>
    /// <param name="request">The HTTP request message to modify.</param>
    /// <param name="apiKey">The API key used to authenticate the request.</param>
    void Intercept(HttpRequestMessage request, string apiKey);
}