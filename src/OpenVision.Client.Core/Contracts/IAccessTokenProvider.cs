namespace OpenVision.Client.Core.Contracts;

/// <summary>
/// Provides a mechanism for retrieving the current access token from the HTTP context.
/// </summary>
public interface IAccessTokenProvider
{
    /// <summary>
    /// Gets the access token for the current HTTP request.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation, containing the access token as a string.
    /// </returns>
    Task<string?> GetAccessTokenAsync();
}
