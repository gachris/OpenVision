using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenVision.Client.Core.Configuration;
using OpenVision.Client.Core.Contracts;

namespace OpenVision.Client.Core.Services;

/// <summary>
/// Service for creating an HttpClient for making requests to a cloud API using IHttpClientFactory.
/// </summary>
/// <remarks>
/// This service leverages the ASP.NET Core IHttpClientFactory interface to generate HttpClient instances,
/// ensuring optimal resource management and connection reuse.
/// The returned HttpClient's BaseAddress is set using the <see cref="OpenVisionApiOptions"/> configuration.
/// </remarks>
public class OpenVisionApiClientFactory : IOpenVisionApiClientFactory
{
    #region Fields/Consts

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OpenVisionApiOptions _openVisionServerOptions;
    private readonly ILogger<OpenVisionApiClientFactory> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenVisionApiClientFactory"/> class 
    /// with the specified <see cref="IHttpClientFactory"/> and configuration options.
    /// </summary>
    /// <param name="httpClientFactory">The IHttpClientFactory instance used for creating HttpClient instances.</param>
    /// <param name="options">The configuration options containing the cloud API BaseUri.</param>
    /// <param name="logger">The logger instance.</param>
    public OpenVisionApiClientFactory(IHttpClientFactory httpClientFactory, IOptions<OpenVisionApiOptions> options, ILogger<OpenVisionApiClientFactory> logger)
    {
        _httpClientFactory = httpClientFactory;
        _openVisionServerOptions = options.Value;
        _logger = logger;

        _logger.LogInformation("OpenVisionApiClientFactory configured with BaseUri: {BaseUri}", _openVisionServerOptions.BaseUri);
    }

    #region Methods

    /// <inheritdoc/>
    public HttpClient GetClient()
    {
        _logger.LogInformation("Creating a new HttpClient instance for OpenVision API.");
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(_openVisionServerOptions.BaseUri);
        _logger.LogInformation("HttpClient created with BaseAddress: {BaseAddress}", client.BaseAddress);
        return client;
    }

    #endregion
}
