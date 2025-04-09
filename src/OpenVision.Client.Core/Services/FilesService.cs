using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenVision.Shared;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Services;

/// <summary>
/// Provides methods to interact with the API's files endpoint.
/// </summary>
public class FilesService : IFilesService
{
    #region Fields/Consts

    private const string Route = "api/files";

    private readonly HttpContext _httpContext;
    private readonly ICloudHttpClientService _cloudHttpClientService;
    private readonly ILogger<FilesService> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="FilesService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="cloudHttpClientService">The cloud HTTP client service.</param>
    /// <param name="logger">The logger.</param>
    public FilesService(IHttpContextAccessor httpContextAccessor,
                        ICloudHttpClientService cloudHttpClientService,
                        ILogger<FilesService> logger)
    {
        _httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _cloudHttpClientService = cloudHttpClientService;
        _logger = logger;
    }

    #region IFilesService Implementation

    /// <inheritdoc/>
    public async Task<IResponseMessage<DownloadFileResult>> DownloadAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var client = _cloudHttpClientService.GetClient();

        var token = await _httpContext.GetTokenAsync("access_token");
        ArgumentNullException.ThrowIfNull(token, nameof(token));

        client.SetBearerToken(token);

        var response = await client.GetAsync($"{Route}/{id}", cancellationToken);
        var responseContent = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        var filename = response.Content.Headers.ContentDisposition?.FileName ?? "Unknown";
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";

        var downloadFileResult = new DownloadFileResult(filename, responseContent, contentType);
        var responseDoc = new ResponseDoc<DownloadFileResult>(downloadFileResult);
        return new ResponseMessage<DownloadFileResult>(responseDoc, Guid.NewGuid(), StatusCode.Success, []);
    }

    #endregion
}
