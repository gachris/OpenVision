using System.Globalization;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenVision.Client.Core.Configuration;

namespace OpenVision.Client.Core.Middlewares;

/// <summary>
/// Middleware that handles token refresh logic for authenticated users.
/// </summary>
public class TokenRefreshMiddleware
{
    #region Fields/Consts
    
    private readonly RequestDelegate _next;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OidcConfiguration _oidcConfiguration;
    private readonly ILogger<TokenRefreshMiddleware> _logger;

    #endregion
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TokenRefreshMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="httpClientFactory">The HTTP client factory used to create clients.</param>
    /// <param name="oidcConfiguration">The OpenID Connect configuration options.</param>
    /// <param name="logger">The logger instance for diagnostics.</param>
    public TokenRefreshMiddleware(
        RequestDelegate next,
        IHttpClientFactory httpClientFactory,
        OidcConfiguration oidcConfiguration,
        ILogger<TokenRefreshMiddleware> logger)
    {
        _next = next;
        _httpClientFactory = httpClientFactory;
        _oidcConfiguration = oidcConfiguration;
        _logger = logger;
    }

    #region Methods

    /// <summary>
    /// Invokes the middleware to refresh tokens if necessary.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task that represents the middleware processing.</returns>
    public virtual async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            _logger.LogDebug("User is not authenticated, skipping token refresh.");
            await _next(context);
            return;
        }

        var expiresAt = await context.GetTokenAsync("expires_at");

        var expiresAtIsValid = DateTime.TryParse(
            expiresAt,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
            out var expiresAtUtc);

        if (!expiresAtIsValid)
        {
            _logger.LogWarning("Token expiration time is invalid. Skipping token refresh.");
            await _next(context);
            return;
        }

        if (expiresAtUtc < DateTime.UtcNow)
        {
            _logger.LogInformation("Access token has expired, attempting to refresh.");

            var refreshToken = await context.GetTokenAsync("refresh_token");
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogWarning("Refresh token is null or empty. Signing out the user.");
                await context.SignOutAsync();
                await _next(context);
                return;
            }

            var client = _httpClientFactory.CreateClient();
            var disco = await client.GetDiscoveryDocumentAsync(_oidcConfiguration.Authority);
            if (disco.IsError)
            {
                _logger.LogError("Error retrieving discovery document from authority {Authority}: {Error}", _oidcConfiguration.Authority, disco.Error);
                await _next(context);
                return;
            }

            var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _oidcConfiguration.ClientId,
                ClientSecret = _oidcConfiguration.ClientSecret,
                RefreshToken = refreshToken
            });

            if (tokenResponse.IsError)
            {
                _logger.LogError("Error refreshing token: {Error}", tokenResponse.Error);
                await context.SignOutAsync();
            }
            else if (string.IsNullOrEmpty(tokenResponse.AccessToken) ||
                     string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                _logger.LogError("Token response returned null or empty access/refresh token. Signing out user.");
                await context.SignOutAsync();
            }
            else
            {
                _logger.LogInformation("Token refresh succeeded. Updating authentication tokens.");
                var authResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (authResult.Succeeded && authResult.Principal != null)
                {
                    authResult.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
                    authResult.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);
                    var newExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
                    authResult.Properties.UpdateTokenValue("expires_at", newExpiry.ToString("o", CultureInfo.InvariantCulture));

                    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authResult.Principal, authResult.Properties);
                }
                else
                {
                    _logger.LogWarning("Authentication failed during token refresh update.");
                }
            }
        }
        else
        {
            _logger.LogDebug("Access token is still valid. No refresh needed.");
        }

        await _next(context);
    } 

    #endregion
}
