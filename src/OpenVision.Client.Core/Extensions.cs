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
using OpenVision.Client.Core.Configuration.Constants;
using OpenVision.Client.Core.Helpers;
using OpenVision.Client.Core.Localization;
using OpenVision.Client.Core.Middleware;
using OpenVision.Client.Core.Services;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Helper class for configuring ASP.NET Core services and middleware.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds default services and configurations for an OpenVision client application.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance.</param>
    /// <param name="appConfiguration">The application configuration containing authentication and general settings.</param>
    /// <returns>The configured <see cref="IHostApplicationBuilder"/> instance.</returns>
    public static IHostApplicationBuilder AddOpenVisionClientDefaults(this WebApplicationBuilder builder, AppConfiguration appConfiguration)
    {
        // Add application services
        builder.Services.AddApplicationServices(appConfiguration);

        // Add application localization
        builder.Services.AddApplicationLocalization();

        // Add HttpContextAccessor
        builder.Services.AddHttpContextAccessor();

        // Add cookie policy
        builder.Services.AddAppCookiePolicy();

        // Add authentication
        builder.Services.AddAuthentication(appConfiguration);

        // Add application controllers
        builder.Services.AddAppControllers();

        return builder;
    }

    /// <summary>
    /// Adds localization services to the application.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddApplicationLocalization(this IServiceCollection services)
    {
        services.AddLocalization(opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; });
        services.TryAddTransient(typeof(IGenericControllerLocalizer<>), typeof(GenericControllerLocalizer<>));
        services.AddTransient<IViewLocalizer, ResourceViewLocalizer>();

        return services;
    }

    /// <summary>
    /// Adds application-specific services to the service collection.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, AppConfiguration appConfiguration)
    {
        services.AddSingleton(appConfiguration);
        services.AddTransient<IDatabasesService, DatabasesService>();
        services.AddTransient<IFilesService, FilesService>();
        services.AddTransient<ITargetsService, TargetsService>();
        services.AddTransient<ICloudHttpClientService, CloudHttpClientService>();

        return services;
    }

    /// <summary>
    /// Configures MVC controllers and related options.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddAppControllers(this IServiceCollection services)
    {
        services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts => { opts.ResourcesPath = ConfigurationConsts.ResourcesPath; })
                .AddDataAnnotationsLocalization()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                });

        return services;
    }

    /// <summary>
    /// Configures cookie policy options for the application.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddAppCookiePolicy(this IServiceCollection services)
    {
        return services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            options.Secure = CookieSecurePolicy.SameAsRequest;
            options.OnAppendCookie = cookieContext =>
                AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            options.OnDeleteCookie = cookieContext =>
                AuthenticationHelpers.CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
        });
    }

    /// <summary>
    /// Configures authentication and authorization services.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <param name="appConfiguration">The application configuration.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddAuthentication(this IServiceCollection services, AppConfiguration appConfiguration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = AuthenticationConsts.OidcAuthenticationScheme;
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.Name = appConfiguration.IdentityAdminCookieName;
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(appConfiguration.IdentityAdminCookieExpiresUtcHours);
            })
            .AddOpenIdConnect(AuthenticationConsts.OidcAuthenticationScheme, options =>
            {
                options.Authority = appConfiguration.IdentityServerBaseUrl;
                options.RequireHttpsMetadata = appConfiguration.RequireHttpsMetadata;
                options.ClientId = appConfiguration.ClientId;
                options.ClientSecret = appConfiguration.ClientSecret;
                options.ResponseType = appConfiguration.OidcResponseType;

                options.Scope.Clear();
                foreach (var scope in appConfiguration.Scopes)
                {
                    options.Scope.Add(scope);
                }

                options.ClaimActions.MapJsonKey(appConfiguration.TokenValidationClaimRole, appConfiguration.TokenValidationClaimRole, appConfiguration.TokenValidationClaimRole);

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = appConfiguration.TokenValidationClaimName,
                    RoleClaimType = appConfiguration.TokenValidationClaimRole
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Properties ??= new AuthenticationProperties();
                        context.Properties.IsPersistent = true;
                        context.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddHours(appConfiguration.IdentityAdminCookieExpiresUtcHours));

                        return Task.FromResult(0);
                    },
                    OnRedirectToIdentityProvider = context =>
                    {
                        context.ProtocolMessage.RedirectUri = appConfiguration.IdentityAdminRedirectUri;

                        return Task.FromResult(0);
                    }
                };
            });

        return services;
    }

    /// <summary>
    /// Configures default middleware and pipeline settings for the OpenVision client application.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance.</param>
    /// <param name="securityConfiguration">The security configuration settings used for applying security headers such as CSP.</param>
    /// <returns>The configured <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder UseOpenVisionClientDefaults(this WebApplication app, SecurityConfiguration securityConfiguration)
    {
        // Configure development environment settings
        app.ConfigureDevelopmentEnvironment();

        // Enable HTTPS redirection
        app.UseHttpsRedirection();

        // Use security headers
        app.UseSecurityHeaders(securityConfiguration.CspTrustedDomains);

        // Enable static files
        app.UseStaticFiles();

        // Enable routing
        app.UseRouting();

        app.UseCookiePolicy();

        // Use authentication middleware
        app.UseAuthentication();

        app.UseMiddleware<TokenRefreshMiddleware>();

        // Use authorization middleware
        app.UseAuthorization();

        // Map application controllers
        app.MapAppControllers();

        return app;
    }

    /// <summary>
    /// Configures development-specific environment settings.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The configured application builder.</returns>
    public static IApplicationBuilder ConfigureDevelopmentEnvironment(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        return app;
    }

    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app, List<string> cspTrustedDomains)
    {
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

    /// <summary>
    /// Maps the default controller route for the application.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The configured application builder.</returns>
    public static IApplicationBuilder MapAppControllers(this WebApplication app)
    {
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        return app;
    }
}