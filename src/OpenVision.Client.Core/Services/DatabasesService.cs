using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using OpenVision.Client.Core.Requests;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Services;

/// <summary>
/// Provides methods to interact with the API's databases endpoint.
/// </summary>
public class DatabasesService : IDatabasesService
{
    #region Fields/Consts

    private const string Route = "api/databases";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };
    private readonly HttpContext _httpContext;
    private readonly ICloudHttpClientService _cloudHttpClientService;
    private readonly ILogger<DatabasesService> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabasesService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="cloudHttpClientService">The cloud HTTP client service.</param>
    /// <param name="logger">The logger.</param>
    public DatabasesService(IHttpContextAccessor httpContextAccessor,
                            ICloudHttpClientService cloudHttpClientService,
                            ILogger<DatabasesService> logger)
    {
        _httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _cloudHttpClientService = cloudHttpClientService;
        _logger = logger;
    }

    #region IDatabasesService Implementation

    /// <inheritdoc/>
    public async Task<DatabasePagedResponse> GetAsync(DatabaseBrowserQuery query, CancellationToken cancellationToken = default)
    {
        using var client = _cloudHttpClientService.GetClient();

        var token = await _httpContext.GetTokenAsync("access_token");
        client.SetBearerToken(token);

        var queryParams = new List<KeyValuePair<string, StringValues>>()
        {
            new("page", query.Page.ToString()),
            new("size", query.Size.ToString())
        };

        if (!string.IsNullOrEmpty(query.Description))
        {
            queryParams.Add(new KeyValuePair<string, StringValues>("description", query.Description));
        }

        if (query.Created.HasValue)
        {
            queryParams.Add(new KeyValuePair<string, StringValues>("created", query.Created.Value.ToString("yyyy-MM-dd")));
        }

        var requestUrl = QueryHelpers.AddQueryString(Route, queryParams);
        var response = await client.GetAsync(requestUrl, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<DatabasePagedResponse>(JsonSerializerOptions, cancellationToken: cancellationToken);

        return result ?? throw new ArgumentNullException(nameof(result));
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<DatabaseResponse>> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var client = _cloudHttpClientService.GetClient();

        var token = await _httpContext.GetTokenAsync("access_token");
        client.SetBearerToken(token);

        var response = await client.GetAsync($"{Route}/{id}", cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResponseMessage<DatabaseResponse>>(JsonSerializerOptions, cancellationToken: cancellationToken);

        return result ?? throw new ArgumentNullException(nameof(result));
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<DatabaseResponse>> CreateAsync(PostDatabaseRequest body, CancellationToken cancellationToken = default)
    {
        using var client = _cloudHttpClientService.GetClient();

        var token = await _httpContext.GetTokenAsync("access_token");
        client.SetBearerToken(token);

        var response = await client.PostAsJsonAsync(Route, body, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResponseMessage<DatabaseResponse>>(JsonSerializerOptions, cancellationToken: cancellationToken);

        return result ?? throw new ArgumentNullException(nameof(result));
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<DatabaseResponse>> UpdateAsync(Guid id, UpdateDatabaseRequest body, CancellationToken cancellationToken = default)
    {
        using var client = _cloudHttpClientService.GetClient();

        var token = await _httpContext.GetTokenAsync("access_token");
        client.SetBearerToken(token);

        var response = await client.PutAsJsonAsync($"{Route}/{id}", body, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResponseMessage<DatabaseResponse>>(JsonSerializerOptions, cancellationToken: cancellationToken);

        return result ?? throw new ArgumentNullException(nameof(result));
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var client = _cloudHttpClientService.GetClient();

        var token = await _httpContext.GetTokenAsync("access_token");
        client.SetBearerToken(token);

        var response = await client.DeleteAsync($"{Route}/{id}", cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResponseMessage<bool>>(JsonSerializerOptions, cancellationToken: cancellationToken);

        return result ?? throw new ArgumentNullException(nameof(result));
    }

    #endregion
}