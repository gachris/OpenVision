using Duende.IdentityModel.Client;
using Microsoft.Extensions.Logging;
using OpenVision.Client.Core.Contracts;
using OpenVision.Client.Core.Dtos;

namespace OpenVision.Client.Core.Services;

/// <summary>
/// Provides methods to interact with the API's files endpoint.
/// </summary>
public class FileApiService : IFileApiService
{
    #region Fields/Consts

    private const string _route = "api/files";
    private readonly IOpenVisionApiClientFactory _openVisionHttpClientService;
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly ILogger<FileApiService> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="FileApiService"/> class.
    /// </summary>
    /// <param name="openVisionHttpClientService">The OpenVision API HTTP client factory.</param>
    /// <param name="accessTokenProvider">The access token provider.</param>
    /// <param name="logger">The logger instance.</param>
    public FileApiService(
        IOpenVisionApiClientFactory openVisionHttpClientService,
        IAccessTokenProvider accessTokenProvider,
        ILogger<FileApiService> logger)
    {
        _openVisionHttpClientService = openVisionHttpClientService;
        _accessTokenProvider = accessTokenProvider;
        _logger = logger;
    }

    #region Methods

    /// <inheritdoc/>
    public async Task<DatabaseFileDto> DownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Downloading file with ID: {FileId}", id);

        using var client = _openVisionHttpClientService.GetClient();

        var token = await _accessTokenProvider.GetAccessTokenAsync();
        if (string.IsNullOrEmpty(token))
        {
            _logger.LogError("Access token is missing for file ID: {FileId}", id);
            throw new ArgumentException("Access token is missing.", nameof(token));
        }

        client.SetBearerToken(token);

        var response = await client.GetAsync($"{_route}/{id}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to download file with ID: {FileId}. Status code: {StatusCode}", id, response.StatusCode);
            response.EnsureSuccessStatusCode();
        }

        var fileBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
        var filename = response.Content.Headers.ContentDisposition?.FileName?.Trim('"');
        var contentType = response.Content.Headers.ContentType?.MediaType;

        if (string.IsNullOrEmpty(filename))
        {
            _logger.LogError("Filename is missing in response for file ID: {FileId}", id);
            throw new ArgumentException("Filename is missing.", nameof(filename));
        }
        if (string.IsNullOrEmpty(contentType))
        {
            _logger.LogError("Content type is missing in response for file ID: {FileId}", id);
            throw new ArgumentException("Content type is missing.", nameof(contentType));
        }

        _logger.LogInformation("File downloaded successfully. Filename: {Filename}, ContentType: {ContentType}", filename, contentType);

        return new DatabaseFileDto()
        {
            Filename = filename,
            FileContents = fileBytes,
            ContentType = contentType
        };
    }

    #endregion
}
