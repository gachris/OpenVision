using System;
using System.IO;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenVision.IdentityServer.Admin.EntityFramework.Shared.DbContexts;
using OpenVision.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity;
using OpenVision.IdentityServer.STS.Identity.Configuration;
using OpenVision.IdentityServer.STS.Identity.Configuration.Constants;
using OpenVision.IdentityServer.STS.Identity.Configuration.Interfaces;
using OpenVision.IdentityServer.STS.Identity.Helpers;
using Serilog;
using Skoruba.Duende.IdentityServer.Shared.Configuration.Helpers;

namespace OpenVision.IdentityServer.STS.Identity;

public static class ProgramHelper
{
    /// <summary>
    /// Retrieves the application configuration.
    /// </summary>
    /// <typeparam name="T">
    /// The type whose user secrets are being loaded (typically the entry point type).
    /// </typeparam>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>The application configuration.</returns>
    public static IConfiguration GetConfiguration<T>(string[] args) where T : class
    {
        // Retrieve the current environment from environment variables.
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        // Determine if the current environment is development.
        var isDevelopment = environment == Environments.Development;

        // Create a new configuration builder and set the base path.
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            // Load the main appsettings.json file.
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            // Load environment-specific appsettings file if it exists.
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            // Load Serilog configuration files.
            .AddJsonFile("serilog.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"serilog.{environment}.json", optional: true, reloadOnChange: true);

        // If in development, add user secrets to help keep sensitive data out of source control.
        if (isDevelopment)
        {
            configurationBuilder.AddUserSecrets<T>(optional: true);
        }

        var configuration = configurationBuilder.Build();

        configuration.AddAzureKeyVaultConfiguration(configurationBuilder);

        // Allow command-line arguments and environment variables to override configuration values.
        configurationBuilder.AddCommandLine(args);
        configurationBuilder.AddEnvironmentVariables();

        // Build and return the final configuration.
        return configurationBuilder.Build();
    }

    /// <summary>
    /// Configures the host builder for the application.
    /// </summary>
    /// <typeparam name="T">The type whose user secrets are being loaded (typically the entry point type).</typeparam>
    /// <param name="builder">The WebApplicationBuilder instance.</param>
    /// <param name="args">Command-line arguments passed to the application.</param>
    public static void ConfigureHostBuilder<T>(this WebApplicationBuilder builder, string[] args) where T : class
    {
        // Add Serilog configuration files.
        builder.Configuration.AddJsonFile("serilog.json", optional: true, reloadOnChange: true);
        var env = builder.Environment;
        builder.Configuration.AddJsonFile($"serilog.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

        // In development, add user secrets.
        if (env.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets<T>(optional: true);
        }

        builder.Configuration.AddAzureKeyVaultConfiguration(builder.Configuration);

        // Add environment variables and command-line arguments.
        builder.Configuration.AddEnvironmentVariables();
        builder.Configuration.AddCommandLine(args);

        builder.WebHost.UseStaticWebAssets();
        // Configure Kestrel to not include the server header in responses.
        builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

        // Configure the host to use Serilog for logging.
        builder.Host.UseSerilog((hostContext, loggerConfig) =>
        {
            loggerConfig
                .ReadFrom.Configuration(hostContext.Configuration)
                // Enrich logs with the application name.
                .Enrich.WithProperty("ApplicationName", hostContext.HostingEnvironment.ApplicationName);
        });
    }

    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var rootConfiguration = CreateRootConfiguration(configuration);
        services.AddSingleton(rootConfiguration);
        // Register DbContexts for IdentityServer and Identity
        RegisterDbContexts(services, configuration);

        // Save data protection keys to db, using a common application name shared between Admin and STS
        services.AddDataProtection<IdentityServerDataProtectionDbContext>(configuration);

        // Add email senders which is currently setup for SendGrid and SMTP
        services.AddEmailSenders(configuration);

        // Add services for authentication, including Identity model and external providers
        RegisterAuthentication(services, configuration);

        // Add HSTS options
        RegisterHstsOptions(services);

        // Add all dependencies for Asp.Net Core Identity in MVC - these dependencies are injected into generic Controllers
        // Including settings for MVC and Localization
        // If you want to change primary keys or use another db model for Asp.Net Core Identity:
        services.AddMvcWithLocalization<UserIdentity, string>(configuration);

        // Add authorization policies for MVC
        RegisterAuthorization(services, configuration);

        services.AddIdSHealthChecks<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminIdentityDbContext, IdentityServerDataProtectionDbContext>(configuration);
    }

    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
    {
        app.UseCookiePolicy();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UsePathBase(configuration.GetValue<string>("BasePath"));


        app.UseStaticFiles();
        UseAuthentication(app);

        // Add custom security headers
        app.UseSecurityHeaders(configuration);

        app.UseMvcLocalizationServices();

        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoint =>
        {
            endpoint.MapDefaultControllerRoute();
            endpoint.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        });
    }

    public static void RegisterDbContexts(IServiceCollection services, IConfiguration configuration)
    {
        services.RegisterDbContexts<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, IdentityServerDataProtectionDbContext>(configuration);
    }

    public static void RegisterAuthentication(IServiceCollection services, IConfiguration Configuration)
    {
        services.AddAuthenticationServices<AdminIdentityDbContext, UserIdentity, UserIdentityRole>(Configuration);
        services.AddIdentityServer<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, UserIdentity>(Configuration);
    }

    public static void RegisterAuthorization(IServiceCollection services, IConfiguration configuration)
    {
        var rootConfiguration = CreateRootConfiguration(configuration);
        services.AddAuthorizationPolicies(rootConfiguration);
    }

    public static void UseAuthentication(IApplicationBuilder app)
    {
        app.UseIdentityServer();
    }

    public static void RegisterHstsOptions(IServiceCollection services)
    {
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });
    }

    private static IRootConfiguration CreateRootConfiguration(IConfiguration configuration)
    {
        var rootConfiguration = new RootConfiguration();
        configuration.GetSection(ConfigurationConsts.AdminConfigurationKey).Bind(rootConfiguration.AdminConfiguration);
        configuration.GetSection(ConfigurationConsts.RegisterConfigurationKey).Bind(rootConfiguration.RegisterConfiguration);
        return rootConfiguration;
    }
}
