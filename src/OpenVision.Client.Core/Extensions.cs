using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using NWebsec.Core.Common.HttpHeaders.Configuration;
using NWebsec.Core.Common.Middleware.Options;
using OpenVision.Client.Core.Configuration;
using OpenVision.Client.Core.Constants;
using OpenVision.Client.Core.Helpers;
using OpenVision.Client.Core.Localization;
using OpenVision.Client.Core.Services;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Contains extension methods for configuring the OpenVision Client application.
/// This class centralizes the registration and configuration of common services, middleware, 
/// authentication, authorization, application-specific features. 
/// Using these extension methods ensures that the application is 
/// consistently configured across different environments.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds localization services to the application.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddDefaultLocalization(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddTransient(typeof(IGenericControllerLocalizer<>), typeof(GenericControllerLocalizer<>));
        services.AddTransient<IViewLocalizer, ResourceViewLocalizer>();
        services.AddLocalization(options =>
        {
            options.ResourcesPath = ConfigurationConsts.ResourcesPath;
        });

        return services;
    }

    /// <summary>
    /// Adds application-specific services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The modified service collection with OpenVision services registered.</returns>
    public static IServiceCollection AddOpenVisionServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddTransient<IDatabasesService, DatabasesService>();
        services.AddTransient<IFilesService, FilesService>();
        services.AddTransient<ITargetsService, TargetsService>();
        services.AddTransient<ICloudHttpClientService, CloudHttpClientService>();
        return services;
    }

    /// <summary>
    /// Configures cookie policy options for the application.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The modified service collection with cookie policy options configured.</returns>
    public static IServiceCollection AddDefaultCookiePolicy(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            options.Secure = CookieSecurePolicy.SameAsRequest;
            options.OnAppendCookie = cookieContext => AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            options.OnDeleteCookie = cookieContext => AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
        });
    }

    /// <summary>
    /// Configures Identity Server authentication using cookies and OpenID Connect.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="oidcConfiguration">The oidc configuration settings for Identity Server.</param>
    /// <returns>The updated AuthenticationBuilder instance.</returns>
    public static AuthenticationBuilder AddIdentityServerAuthentication(this IServiceCollection services, OidcConfiguration oidcConfiguration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(oidcConfiguration);

        services.AddSingleton(oidcConfiguration);

        var authenticationBuilder = services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = AuthenticationConsts.OidcAuthenticationScheme;
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        });
        
        authenticationBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.Cookie.Name = oidcConfiguration.CookieName;
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromHours(oidcConfiguration.CookieExpiresUtcHours);
        });

        authenticationBuilder.AddOpenIdConnect(AuthenticationConsts.OidcAuthenticationScheme, options =>
        {
            options.Authority = oidcConfiguration.Authority;
            options.RequireHttpsMetadata = oidcConfiguration.RequireHttpsMetadata;
            options.ClientId = oidcConfiguration.ClientId;
            options.ClientSecret = oidcConfiguration.ClientSecret;
            options.ResponseType = oidcConfiguration.ResponseType;

            options.Scope.Clear();
            foreach (var scope in oidcConfiguration.Scopes)
            {
                options.Scope.Add(scope);
            }

            options.ClaimActions.MapJsonKey(oidcConfiguration.TokenValidationClaimRole, oidcConfiguration.TokenValidationClaimRole, oidcConfiguration.TokenValidationClaimRole);

            options.SaveTokens = true;
            options.GetClaimsFromUserInfoEndpoint = true;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = oidcConfiguration.TokenValidationClaimName,
                RoleClaimType = oidcConfiguration.TokenValidationClaimRole
            };

            options.Events = new OpenIdConnectEvents
            {
                OnMessageReceived = context =>
                {
                    context.Properties ??= new AuthenticationProperties();
                    context.Properties.IsPersistent = true;
                    context.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddHours(oidcConfiguration.CookieExpiresUtcHours));

                    return Task.FromResult(0);
                },
                OnRedirectToIdentityProvider = context =>
                {
                    context.ProtocolMessage.RedirectUri = oidcConfiguration.RedirectUri;

                    return Task.FromResult(0);
                }
            };
        });

        return authenticationBuilder;
    }

    /// <summary>
    /// Adds view localization services with default settings.
    /// </summary>
    /// <param name="mvcBuilder">The MVC builder to which the localization services will be added.</param>
    /// <returns>The updated IMvcBuilder instance.</returns>
    public static IMvcBuilder AddDefaultViewLocalization(this IMvcBuilder mvcBuilder)
    {
        ArgumentNullException.ThrowIfNull(mvcBuilder);

        // Configure view localization to use the specified resource path and suffix format.
        mvcBuilder.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts =>
        {
            opts.ResourcesPath = ConfigurationConsts.ResourcesPath;
        });

        return mvcBuilder;
    }

    /// <summary>
    /// Configures default JSON serialization options for controllers.
    /// </summary>
    /// <param name="mvcBuilder">The MVC builder.</param>
    /// <returns>The updated IMvcBuilder.</returns>
    public static IMvcBuilder AddDefaultJsonOptions(this IMvcBuilder mvcBuilder)
    {
        ArgumentNullException.ThrowIfNull(mvcBuilder);

        mvcBuilder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });

        return mvcBuilder;
    }

    /// <summary>
    /// Configures security headers for the application, including CSP and other headers.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="cspTrustedDomains">A list of trusted domains for Content Security Policy.</param>
    /// <returns>The updated IApplicationBuilder instance with security headers applied.</returns>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, List<string> cspTrustedDomains)
    {
        ArgumentNullException.ThrowIfNull(app);

        var forwardedHeadersOptions = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.All
        };
        forwardedHeadersOptions.KnownNetworks.Clear();
        forwardedHeadersOptions.KnownProxies.Clear();
        app.UseForwardedHeaders(forwardedHeadersOptions);
        app.UseXXssProtection(delegate (IFluentXXssProtectionOptions options)
        {
            options.EnabledWithBlockMode();
        });
        app.UseXContentTypeOptions();
        app.UseXfo(delegate (IFluentXFrameOptions options)
        {
            options.SameOrigin();
        });
        app.UseReferrerPolicy(delegate (IFluentReferrerPolicyOptions options)
        {
            options.NoReferrer();
        });
        if (cspTrustedDomains == null || cspTrustedDomains.Count == 0)
        {
            return app;
        }

        app.UseCsp(delegate (IFluentCspOptions csp)
        {
            List<string> imagesCustomSources = [.. cspTrustedDomains, "data:"];
            csp.ImageSources(delegate (ICspDirectiveBasicConfiguration options)
            {
                options.SelfSrc = true;
                options.CustomSources = imagesCustomSources;
                options.Enabled = true;
            });
            csp.FontSources(delegate (ICspDirectiveBasicConfiguration options)
            {
                options.SelfSrc = true;
                options.CustomSources = cspTrustedDomains;
                options.Enabled = true;
            });
            csp.ScriptSources(delegate (ICspDirectiveConfiguration options)
            {
                options.SelfSrc = true;
                options.CustomSources = cspTrustedDomains;
                options.Enabled = true;
                options.UnsafeInlineSrc = true;
                options.UnsafeEvalSrc = true;
            });
            csp.StyleSources(delegate (ICspDirectiveUnsafeInlineConfiguration options)
            {
                options.SelfSrc = true;
                options.CustomSources = cspTrustedDomains;
                options.Enabled = true;
                options.UnsafeInlineSrc = true;
            });
            csp.DefaultSources(delegate (ICspDirectiveBasicConfiguration options)
            {
                options.SelfSrc = true;
                options.CustomSources = cspTrustedDomains;
                options.Enabled = true;
            });
        });

        return app;
    }
}