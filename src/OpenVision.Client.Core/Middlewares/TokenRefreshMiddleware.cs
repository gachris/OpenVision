using System.Globalization;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using OpenVision.Client.Core.Configuration;

namespace OpenVision.Client.Core.Middlewares;

public class TokenRefreshMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OidcConfiguration _oidcConfiguration;

    public TokenRefreshMiddleware(
        RequestDelegate next,
        IHttpClientFactory httpClientFactory,
        OidcConfiguration oidcConfiguration)
    {
        _next = next;
        _httpClientFactory = httpClientFactory;
        _oidcConfiguration = oidcConfiguration;
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
            var disco = await client.GetDiscoveryDocumentAsync(_oidcConfiguration.Authority);

            if (disco.IsError)
            {
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
