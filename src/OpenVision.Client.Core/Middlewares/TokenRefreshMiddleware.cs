using System.Globalization;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using OpenVision.Client.Core.Configuration;

namespace OpenVision.Client.Core.Middleware;

public class TokenRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppConfiguration _appConfiguration;

    public TokenRefreshMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, AppConfiguration appConfiguration)
    {
        _next = next;
        _httpClientFactory = httpClientFactory;
        _appConfiguration = appConfiguration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            await _next(context);
            return;
        }

        var expiresAt = await context.GetTokenAsync("expires_at");

        var expiresAtIsValid = DateTime.TryParse(
            expiresAt,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
            out var expiresAtUtc);

        if (expiresAtIsValid && expiresAtUtc < DateTime.UtcNow)
        {
            var refreshToken = await context.GetTokenAsync("refresh_token");

            var client = _httpClientFactory.CreateClient();
            var disco = await client.GetDiscoveryDocumentAsync(_appConfiguration.IdentityServerBaseUrl);

            if (disco.IsError)
            {
                await _next(context);
                return;
            }

            var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = _appConfiguration.ClientId,
                ClientSecret = _appConfiguration.ClientSecret,
                RefreshToken = refreshToken
            });

            if (!tokenResponse.IsError)
            {
                var authResult = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                if (authResult.Succeeded && authResult.Principal != null)
                {
                    authResult.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
                    authResult.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);
                    authResult.Properties.UpdateTokenValue("expires_at", DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn).ToString("o"));

                    await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authResult.Principal, authResult.Properties);
                }
            }
            else
            {
                // Optionally handle refresh failure (e.g., log out user)
                await context.SignOutAsync();
            }
        }

        await _next(context);
    }
}
