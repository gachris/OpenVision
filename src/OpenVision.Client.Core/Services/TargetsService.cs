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
/// Provides a service for interacting with the API's target endpoints.
/// </summary>
public class TargetsService : ITargetsService
{
    #region Fields/Consts

    private const string Route = "api/targets";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    private readonly ICloudHttpClientService _cloudHttpClientService;
    private readonly HttpContext _httpContext;
    private readonly ILogger<TargetsService> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetsService"/> class.
    /// </summary>
    /// <param name="cloudHttpClientService">The cloud HTTP client service.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="logger">The logger.</param>
    public TargetsService(IHttpContextAccessor httpContextAccessor,
                          ICloudHttpClientService cloudHttpClientService,
                          ILogger<TargetsService> logger)
    {
        _httpContext = httpContextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _cloudHttpClientService = cloudHttpClientService;
        _logger = logger;
    }

    #region ITargetsService Implementation

    /// <inheritdoc/>
    public async Task<TargetPagedResponse> GetAsync(TargetBrowserQuery query, CancellationToken cancellationToken = default)
    {
        using var client = _cloudHttpClientService.GetClient();

        var token = await _httpContext.GetTokenAsync("access_token");
        client.SetBearerToken(token);

        var queryParams = new List<KeyValuePair<string, StringValues>>()
        {
            new KeyValuePair<string, StringValues>("page", query.Page.ToString()),
            new KeyValuePair<string, StringValues>("size", query.Size.ToString())
        };

        if (!string.IsNullOrEmpty(query.Name))
        {
            queryParams.Add(new KeyValuePair<string, StringValues>("description", query.Name));
        }

        if (query.Created.HasValue)
        {
            queryParams.Add(new KeyValuePair<string, StringValues>("created", query.Created.Value.ToString("yyyy-MM-dd")));
        }

        if (query.DatabaseId.HasValue)
        {
            queryParams.Add(new KeyValuePair<string, StringValues>("database_id", $"{query.DatabaseId}"));
        }

        var requestUrl = QueryHelpers.AddQueryString(Route, queryParams);

        var response = await client.GetAsync(requestUrl, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<TargetPagedResponse>(JsonSerializerOptions, cancellationToken: cancellationToken);

        return result ?? throw new ArgumentNullException(nameof(result));
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<TargetResponse>> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var client = _cloudHttpClientService.GetClient();

        var token = await _httpContext.GetTokenAsync("access_token");
        client.SetBearerToken(token);

        var response = await client.GetAsync($"{Route}/{id}", cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResponseMessage<TargetResponse>>(JsonSerializerOptions, cancellationToken: cancellationToken);

        return result ?? throw new ArgumentNullException(nameof(result));
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<TargetResponse>> CreateAsync(PostTargetRequest body, CancellationToken cancellationToken = default)
    {
        using var client = _cloudHttpClientService.GetClient();

        var token = await _httpContext.GetTokenAsync("access_token");
        client.SetBearerToken(token);

        var response = await client.PostAsJsonAsync(Route, body, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResponseMessage<TargetResponse>>(JsonSerializerOptions, cancellationToken: cancellationToken);

        return result ?? throw new ArgumentNullException(nameof(result));
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<TargetResponse>> UpdateAsync(Guid id, UpdateTargetRequest body, CancellationToken cancellationToken = default)
    {
        using var client = _cloudHttpClientService.GetClient();

        var token = await _httpContext.GetTokenAsync("access_token");
        client.SetBearerToken(token);

        var response = await client.PutAsJsonAsync($"{Route}/{id}", body, cancellationToken);
        var result = await response.Content.ReadFromJsonAsync<ResponseMessage<TargetResponse>>(JsonSerializerOptions, cancellationToken: cancellationToken);

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