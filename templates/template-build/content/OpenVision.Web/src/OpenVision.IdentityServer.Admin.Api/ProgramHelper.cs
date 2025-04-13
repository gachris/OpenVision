using System;
using System.IO;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag.AspNetCore;
using OpenVision.IdentityServer.Admin.Api.Configuration;
using OpenVision.IdentityServer.Admin.EntityFramework.Shared.DbContexts;
using OpenVision.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity;
using OpenVision.IdentityServer.Shared.Dtos;
using OpenVision.IdentityServer.Shared.Dtos.Identity;
using Serilog;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.Duende.IdentityServer.Admin.UI.Api.Configuration;
using Skoruba.Duende.IdentityServer.Admin.UI.Api.Helpers;
using Skoruba.Duende.IdentityServer.Shared.Configuration.Helpers;

namespace OpenVision.IdentityServer.Admin.Api;

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
        var adminApiConfiguration = configuration.GetSection(nameof(AdminApiConfiguration)).Get<AdminApiConfiguration>();
        services.AddSingleton(adminApiConfiguration);

        // Add DbContexts
        RegisterDbContexts(services, configuration);

        // Add email senders which is currently setup for SendGrid and SMTP
        services.AddEmailSenders(configuration);

        // Add authentication services
        RegisterAuthentication(services, configuration);

        // Add authorization services
        RegisterAuthorization(services);

        services.AddIdentityServerAdminApi<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, IdentityServerDataProtectionDbContext, AdminLogDbContext, AdminAuditLogDbContext, AuditLog,
            IdentityUserDto, IdentityRoleDto, UserIdentity, UserIdentityRole, string, UserIdentityUserClaim, UserIdentityUserRole,
            UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken,
            IdentityUsersDto, IdentityRolesDto, IdentityUserRolesDto,
            IdentityUserClaimsDto, IdentityUserProviderDto, IdentityUserProvidersDto, IdentityUserChangePasswordDto,
            IdentityRoleClaimsDto, IdentityUserClaimDto, IdentityRoleClaimDto>(configuration, adminApiConfiguration);

        services.AddSwaggerServices(adminApiConfiguration);

        services.AddIdSHealthChecks<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminIdentityDbContext, AdminLogDbContext, AdminAuditLogDbContext, IdentityServerDataProtectionDbContext>(configuration, adminApiConfiguration);
    }

    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
    {
        var adminApiConfiguration = configuration.GetSection(nameof(AdminApiConfiguration)).Get<AdminApiConfiguration>();
        app.AddForwardHeaders();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseOpenApi();
        app.UseSwaggerUi(settings =>
        {
            settings.OAuth2Client = new OAuth2ClientSettings
            {
                ClientId = adminApiConfiguration.OidcSwaggerUIClientId,
                AppName = adminApiConfiguration.ApiName,
                UsePkceWithAuthorizationCodeGrant = true,
                ClientSecret = null
            };
        });

        app.UseRouting();
        UseAuthentication(app);
        app.UseCors();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        });
    }

    public static void RegisterDbContexts(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContexts<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext, IdentityServerDataProtectionDbContext, AuditLog>(configuration);
    }

    public static void RegisterAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.AddApiAuthentication<AdminIdentityDbContext, UserIdentity, UserIdentityRole>(configuration);
    }

    public static void RegisterAuthorization(IServiceCollection services)
    {
        services.AddAuthorizationPolicies();
    }

    public static void UseAuthentication(IApplicationBuilder app)
    {
        app.UseAuthentication();
    }
}
