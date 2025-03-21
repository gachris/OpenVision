namespace OpenVision.Api.Auth;

/// <summary>
/// Provides access methods for authenticating with a web service.
/// </summary>
public class WebServiceAuthentication
{
    /// <summary>
    /// Represents an access method that adds an API key to the authorization header of an HTTP request.
    /// </summary>
    public class AuthorizationHeaderAccessMethod : IAccessMethod
    {
        private const string Schema = "X-API-KEY";

        /// <summary>
        /// Intercepts an HTTP request and adds an API key to its authorization header.
        /// </summary>
        /// <param name="request">The HTTP request message to modify.</param>
        /// <param name="apiKey">The API key to add to the authorization header.</param>
        public void Intercept(HttpRequestMessage request, string apiKey)
        {
            request.Headers.Add(Schema, apiKey);
        }
    }
}
