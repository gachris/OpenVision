using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using OpenVision.Client.Core.Configuration;
using OpenVision.Client.Core.Configuration.Constants;
using OpenVision.Client.Core.Helpers;
using OpenVision.Client.Core.Localization;
using OpenVision.Client.Core.Services;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Helper class for configuring ASP.NET Core services and middleware.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds localization services to the application.
    /// </summary>
    /// <param name="services">The collection of services.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddAppLocalization(this IServiceCollection services)
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
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        var appConfiguration = configuration.GetSection(nameof(AppConfiguration)).Get<AppConfiguration>()!;

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
    public static IServiceCollection AddAppAuthentication(this IServiceCollection services, AppConfiguration appConfiguration)
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

    /// <summary>
    /// Maps the default controller route for the application.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The configured application builder.</returns>
    public static IApplicationBuilder MapAppControllerRoute(this WebApplication app)
    {
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        return app;
    }
}
