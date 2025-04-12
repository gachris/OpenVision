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
/// Provides methods to interact with the API's databases endpoint.
/// </summary>
public class DatabaseApiService : IDatabaseApiService
{
    #region Fields/Consts

    private const string _route = "api/databases";

    private readonly IOpenVisionApiClientFactory _openVisionHttpClientService;
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly ILogger<DatabaseApiService> _logger;

    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseApiService"/> class.
    /// </summary>
    /// <param name="openVisionHttpClientService">The OpenVision API HTTP client factory.</param>
    /// <param name="accessTokenProvider">The access token provider.</param>
    /// <param name="logger">The logger.</param>
    public DatabaseApiService(
        IOpenVisionApiClientFactory openVisionHttpClientService,
        IAccessTokenProvider accessTokenProvider,
        ILogger<DatabaseApiService> logger)
    {
        _openVisionHttpClientService = openVisionHttpClientService;
        _accessTokenProvider = accessTokenProvider;
        _logger = logger;
    }

    #region Methods

    /// <inheritdoc/>
    public async Task<IPagedResponse<IEnumerable<DatabaseResponse>>> GetAsync(DatabaseBrowserQuery query, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving databases with query: page={Page}, size={Size}, name='{Name}'.", query.Page, query.Size, query.Name);

        using var client = _openVisionHttpClientService.GetClient();
        var token = await _accessTokenProvider.GetAccessTokenAsync();
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));
        client.SetBearerToken(token);

        var queryParams = new List<KeyValuePair<string, StringValues>>
        {
            new("page", query.Page.ToString()),
            new("size", query.Size.ToString())
        };

        if (!string.IsNullOrEmpty(query.Name))
        {
            queryParams.Add(new KeyValuePair<string, StringValues>("name", query.Name));
        }

        var requestUrl = QueryHelpers.AddQueryString(_route, queryParams);
        _logger.LogInformation("Sending GET request to {RequestUrl}.", requestUrl);

        var response = await client.GetAsync(requestUrl, cancellationToken);
        var pagedResult = await response.ReadPagedResponseAsync<IEnumerable<DatabaseResponse>>(cancellationToken: cancellationToken);

        _logger.LogInformation("Retrieved databases successfully.");
        return pagedResult;
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<DatabaseResponse>> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving database with ID: {DatabaseId}.", id);

        using var client = _openVisionHttpClientService.GetClient();
        var token = await _accessTokenProvider.GetAccessTokenAsync();
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));
        client.SetBearerToken(token);

        var requestUrl = $"{_route}/{id}";
        _logger.LogInformation("Sending GET request to {RequestUrl}.", requestUrl);

        var response = await client.GetAsync(requestUrl, cancellationToken);
        var result = await response.ReadResponseMessageAsync<DatabaseResponse>(cancellationToken: cancellationToken);

        _logger.LogInformation("Retrieved database with ID: {DatabaseId} successfully.", id);
        return result;
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<DatabaseResponse>> CreateAsync(PostDatabaseRequest body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new database with Name: {Name}.", body.Name);

        using var client = _openVisionHttpClientService.GetClient();
        var token = await _accessTokenProvider.GetAccessTokenAsync();
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));
        client.SetBearerToken(token);

        var response = await client.PostAsJsonAsync(_route, body, cancellationToken);
        var result = await response.ReadResponseMessageAsync<DatabaseResponse>(cancellationToken: cancellationToken);

        _logger.LogInformation("Successfully created database with Name: {Name}.", body.Name);
        return result;
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<DatabaseResponse>> UpdateAsync(Guid id, UpdateDatabaseRequest body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating database with ID: {DatabaseId}.", id);

        using var client = _openVisionHttpClientService.GetClient();
        var token = await _accessTokenProvider.GetAccessTokenAsync();
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));
        client.SetBearerToken(token);

        var response = await client.PutAsJsonAsync($"{_route}/{id}", body, cancellationToken);
        var result = await response.ReadResponseMessageAsync<DatabaseResponse>(cancellationToken: cancellationToken);

        _logger.LogInformation("Successfully updated database with ID: {DatabaseId}.", id);
        return result;
    }

    /// <inheritdoc/>
    public async Task<IResponseMessage<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting database with ID: {DatabaseId}.", id);

        using var client = _openVisionHttpClientService.GetClient();
        var token = await _accessTokenProvider.GetAccessTokenAsync();
        ArgumentException.ThrowIfNullOrEmpty(token, nameof(token));
        client.SetBearerToken(token);

        var response = await client.DeleteAsync($"{_route}/{id}", cancellationToken);
        var result = await response.ReadResponseMessageAsync<bool>(cancellationToken: cancellationToken);

        _logger.LogInformation("Successfully deleted database with ID: {DatabaseId}.", id);
        return result;
    }

    #endregion
}
