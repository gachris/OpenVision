namespace OpenVision.Client.Core.Services;

/// <summary>
/// Interface for a service that provides an HttpClient configured for use with a cloud API.
/// </summary>
public interface ICloudHttpClientService
{
    /// <summary>
    /// Returns an HttpClient configured for use with a cloud API.
    /// </summary>
    /// <returns>An HttpClient configured for use with a cloud API.</returns>
    HttpClient GetClient();
}