using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenVision.Server.Core.Contracts;

namespace OpenVision.Server.Core.Auth;

/// <summary>
/// Authentication handler for API key-based authentication.
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    #region Fields/Consts

    private readonly IApiKeysRepository _apiKeysRepository;

    #endregion

    /// <summary>
    /// Constructor for ApiKeyAuthenticationHandler.
    /// </summary>
    /// <param name="options">The monitor for the authentication scheme options.</param>
    /// <param name="logger">The logger factory.</param>
    /// <param name="encoder">The URL encoder.</param>
    /// <param name="apiKeysRepository">The repository for accessing api keys.</param>
    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeysRepository apiKeysRepository) : base(options, logger, encoder)
    {
        _apiKeysRepository = apiKeysRepository;
    }

    #region Methods Overrides

    /// <summary>
    /// Handles authentication based on the API key provided in the request header.
    /// </summary>
    /// <returns>An <see cref="AuthenticateResult"/> representing the authentication result.</returns>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyDefaults.X_API_KEY, out var apiKeyHeaders))
        {
            return AuthenticateResult.Fail("API key not found in the request header");
        }

        var apiKeyHeader = apiKeyHeaders.FirstOrDefault();

        if (string.IsNullOrEmpty(apiKeyHeader))
        {
            return AuthenticateResult.Fail("API key not found in the request header");
        }

        try
        {
            var apiKeysQueryable = await _apiKeysRepository.GetAsync();
            var apiKey = await apiKeysQueryable.FirstOrDefaultAsync(apiKey => apiKey.Key == apiKeyHeader);

            if (apiKey == null)
            {
                return AuthenticateResult.Fail("Invalid API key");
            }

            var apiKeyType = apiKey.Type.ToString();

            var claims = new[]
            {
                new Claim(ApiKeyDefaults.X_API_KEY, apiKeyHeader),
                new Claim(ClaimTypes.Role, apiKeyType)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during API key authentication");
            return AuthenticateResult.Fail("Error during authentication");
        }
    }

    #endregion
}