using OpenVision.Client.Core.Configuration;
using OpenVision.Client.Core.Contracts;

namespace OpenVision.Client.Core.Services;

/// <summary>
/// Service for creating an HttpClient for making requests to a cloud API.
/// </summary>
/// <remarks>
/// The HttpClient created by this service has its BaseAddress set to the CloudApiUrl specified in AppConfiguration.
/// </remarks>
public class CloudHttpClientService : ICloudHttpClientService
{
    #region Fields/Consts

    private readonly AppConfiguration _appConfiguration;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="CloudHttpClientService"/> class with the specified <see cref="AppConfiguration"/>.
    /// </summary>
    /// <param name="appConfiguration">The <see cref="AppConfiguration"/> to use.</param>
    public CloudHttpClientService(AppConfiguration appConfiguration)
    {
        _appConfiguration = appConfiguration;
    }

    #region Methods

    /// <inheritdoc/>
    public HttpClient GetClient()
    {
        var handler = new HttpClientHandler();
        return new HttpClient(handler)
        {
            BaseAddress = new Uri(_appConfiguration.OpenVisionServerUrl)
        };
    }

    #endregion
}
