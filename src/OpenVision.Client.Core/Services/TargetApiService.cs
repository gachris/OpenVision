using System.Net.Http.Json;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using OpenVision.Client.Core.Contracts;
using OpenVision.Shared.Extensions;
using OpenVision.Shared.Requests;
using OpenVision.Shared.Responses;

namespace OpenVision.Client.Core.Services;

/// <summary>
/// Provides a service for interacting with the API's target endpoints.
/// </summary>
public class TargetApiService : ITargetApiService
{
    #region Fields/Consts

    private const string _route = "api/targets";
    private readonly IOpenVisionApiClientFactory _openVisionHttpClientService;
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly ILogger<TargetApiService> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TargetApiService"/> class.
    /// </summary>
    /// <param name="openVisionHttpClientService">The OpenVision API HTTP client factory.</param>
    /// <param name="accessTokenProvider">The access token provider.</param>
    /// <param name="logger">The logger instance.</param>
    public TargetApiService(
        IOpenVisionApiClientFactory openVisionHttpClientService,
        IAccessTokenProvider accessTokenProvider,
        ILogger<TargetApiService> logger)
    {
        _openVisionHttpClientService = openVisionHttpClientService;
        _accessTokenProvider = accessTokenProvider;
        _logger = logger;
    }

    #region Methods

    /// <inheritdoc/>
    public async Task<IPagedResponse<IEnumerable<TargetResponse>>> GetAsync(TargetBrowserQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving targets using query with page={Page}, size={Size}.", query.Page, query.Size);

        using var client = _openVisionHttpClientService.GetClient();

        var token = await _accessTokenProvider.GetAccessTokenAsync();
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));
        client.SetBearerToken(token);

        var queryParams = new List<KeyValuePair<string, StringValues>>()
        {
            new("page", query.Page.ToString()),
            new("size", query.Size.ToString())
        };

        if (query.DatabaseId.HasValue)
        {
            queryParams.Add(new KeyValuePair<string, StringValues>("databaseId", $"{query.DatabaseId}"));
        }

        var requestUrl = QueryHelpers.AddQueryString(_route, queryParams);
        _logger.LogInformation("Sending GET request to {RequestUrl}.", requestUrl);

        var response = await client.GetAsync(requestUrl, cancellationToken);
        var pagedResult = await response.ReadPagedResponseAsync<IEnumerable<TargetResponse>>(cancellationToken: cancellationToken);

        _logger.LogInformation("Retrieved targets successfully.");
        return pagedResult;
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<TargetResponse>> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving target with ID: {TargetId}.", id);

        using var client = _openVisionHttpClientService.GetClient();

        var token = await _accessTokenProvider.GetAccessTokenAsync();
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));
        client.SetBearerToken(token);

        var requestUrl = $"{_route}/{id}";
        _logger.LogInformation("Sending GET request to {RequestUrl}.", requestUrl);

        var response = await client.GetAsync(requestUrl, cancellationToken);
        var result = await response.ReadResponseMessageAsync<TargetResponse>(cancellationToken: cancellationToken);

        _logger.LogInformation("Retrieved target with ID: {TargetId} successfully.", id);
        return result;
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<TargetResponse>> CreateAsync(PostTargetRequest body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new target. Name: {Name}.", body.Name);

        using var client = _openVisionHttpClientService.GetClient();

        var token = await _accessTokenProvider.GetAccessTokenAsync();
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));
        client.SetBearerToken(token);

        var response = await client.PostAsJsonAsync(_route, body, cancellationToken);
        var result = await response.ReadResponseMessageAsync<TargetResponse>(cancellationToken: cancellationToken);

        _logger.LogInformation("Successfully created target with Name: {Name}.", body.Name);
        return result;
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<TargetResponse>> UpdateAsync(Guid id, UpdateTargetRequest body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating target with ID: {TargetId}.", id);

        using var client = _openVisionHttpClientService.GetClient();

        var token = await _accessTokenProvider.GetAccessTokenAsync();
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));
        client.SetBearerToken(token);

        var response = await client.PutAsJsonAsync($"{_route}/{id}", body, cancellationToken);
        var result = await response.ReadResponseMessageAsync<TargetResponse>(cancellationToken: cancellationToken);

        _logger.LogInformation("Successfully updated target with ID: {TargetId}.", id);
        return result;
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting target with ID: {TargetId}.", id);

        using var client = _openVisionHttpClientService.GetClient();

        var token = await _accessTokenProvider.GetAccessTokenAsync();
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));
        client.SetBearerToken(token);

        var response = await client.DeleteAsync($"{_route}/{id}", cancellationToken);
        var result = await response.ReadResponseMessageAsync<bool>(cancellationToken: cancellationToken);

        _logger.LogInformation("Successfully deleted target with ID: {TargetId}.", id);
        return result;
    }

    #endregion
}
