using OpenVision.Api.Core;
using OpenVision.Api.Core.Logger;
using OpenVision.Api.Core.Types;

namespace OpenVision.Api.Auth;

/// <summary>
/// Represents the user credential for accessing a database API key.
/// </summary>
public class UserCredential : ICredential, IConfigurableHttpClientInitializer, IHttpExecuteInterceptor, IHttpUnsuccessfulResponseHandler
{
    /// <summary>
    /// The logger used for this class.
    /// </summary>
    private static readonly ILogger _logger = ApplicationContext.Logger.ForType<UserCredential>();

    /// <summary>
    /// The database API key associated with this user credential.
    /// </summary>
    private readonly DatabaseApiKey _apiKey;

    /// <summary>
    /// The access method used to authenticate the user credential.
    /// </summary>
    public IAccessMethod AccessMethod { get; }

    /// <summary>
    /// Gets the database API key associated with this user credential.
    /// </summary>
    public DatabaseApiKey ApiKey => _apiKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserCredential"/> class with the specified database API key.
    /// </summary>
    /// <param name="databaseApiKey">The database API key associated with this user credential.</param>
    public UserCredential(DatabaseApiKey databaseApiKey)
    {
        _apiKey = databaseApiKey;

        AccessMethod = new WebServiceAuthentication.AuthorizationHeaderAccessMethod();
    }

    /// <summary>
    /// Intercepts an HTTP request and adds authentication headers to it.
    /// </summary>
    /// <param name="request">The HTTP request message to modify.</param>
    /// <param name="taskCancellationToken">A cancellation token that can be used to cancel the HTTP request.</param>
    public async Task InterceptAsync(HttpRequestMessage request, CancellationToken taskCancellationToken)
    {
        AccessMethod.Intercept(request, ApiKey.Key);
        await Task.Delay(0);
    }

    /// <summary>
    /// Initializes the specified HTTP client with this user credential's interceptors and response handler.
    /// </summary>
    /// <param name="httpClient">The HTTP client to configure.</param>
    public void Initialize(ConfigurableHttpClient httpClient)
    {
        httpClient.MessageHandler.AddExecuteInterceptor(this);
        httpClient.MessageHandler.AddUnsuccessfulResponseHandler(this);
    }

    /// <summary>
    /// Handles an unsuccessful HTTP response by returning a task that completes with a boolean value indicating whether the response was handled.
    /// </summary>
    /// <param name="args">The arguments for the response handler.</param>
    /// <returns>A task that completes with a boolean value indicating whether the response was handled.</returns>
    public Task<bool> HandleResponseAsync(HandleUnsuccessfulResponseArgs args)
    {
        return Task.FromResult(false);
    }
}