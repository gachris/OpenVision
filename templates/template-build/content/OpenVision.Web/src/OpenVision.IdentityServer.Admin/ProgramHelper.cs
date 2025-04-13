// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenVision.IdentityServer.Admin.Configuration.Database;
using OpenVision.IdentityServer.Admin.EntityFramework.Shared.DbContexts;
using OpenVision.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity;
using OpenVision.IdentityServer.Admin.Helpers;
using OpenVision.IdentityServer.Shared.Dtos;
using OpenVision.IdentityServer.Shared.Dtos.Identity;
using Serilog;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.Duende.IdentityServer.Admin.UI.Helpers.ApplicationBuilder;
using Skoruba.Duende.IdentityServer.Admin.UI.Helpers.DependencyInjection;
using Skoruba.Duende.IdentityServer.Shared.Configuration.Helpers;

namespace OpenVision.IdentityServer.Admin;

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
        builder.Configuration.AddJsonFile("identitydata.json", optional: true, reloadOnChange: true);
        builder.Configuration.AddJsonFile("identityserverdata.json", optional: true, reloadOnChange: true);

        var env = builder.Environment;

        builder.Configuration.AddJsonFile($"serilog.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
        builder.Configuration.AddJsonFile($"identitydata.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
        builder.Configuration.AddJsonFile($"identityserverdata.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

        // In development, add user secrets.
        if (env.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets<T>(optional: true);
        }

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

    public static void ConfigureServices(IServiceCollection services, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
    {
        // Adds the Duende IdentityServer Admin UI with custom options.
        services.AddIdentityServerAdminUI<AdminIdentityDbContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext,
        AdminLogDbContext, AdminAuditLogDbContext, AuditLog, IdentityServerDataProtectionDbContext,
            UserIdentity, UserIdentityRole, UserIdentityUserClaim, UserIdentityUserRole,
            UserIdentityUserLogin, UserIdentityRoleClaim, UserIdentityUserToken, string,
            IdentityUserDto, IdentityRoleDto, IdentityUsersDto, IdentityRolesDto, IdentityUserRolesDto,
            IdentityUserClaimsDto, IdentityUserProviderDto, IdentityUserProvidersDto, IdentityUserChangePasswordDto,
            IdentityRoleClaimsDto, IdentityUserClaimDto, IdentityRoleClaimDto>(options =>
            {
                ConfigureUIOptions(options, webHostEnvironment, configuration);
            });

        // Monitor changes in Admin UI views
        services.AddAdminUIRazorRuntimeCompilation(webHostEnvironment);

        // Add email senders which is currently setup for SendGrid and SMTP
        services.AddEmailSenders(configuration);
    }

    public static void Configure(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseIdentityServerAdminUI();

        app.UseEndpoints(endpoint =>
        {
            endpoint.MapIdentityServerAdminUI();
            endpoint.MapIdentityServerAdminUIHealthChecks();
        });
    }

    public static void ConfigureUIOptions(IdentityServerAdminUIOptions options, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
    {
        // Applies configuration from appsettings.
        options.BindConfiguration(configuration);
        if (webHostEnvironment.IsDevelopment())
        {
            options.Security.UseDeveloperExceptionPage = true;
        }
        else
        {
            options.Security.UseHsts = true;
        }

        // Set migration assembly for application of db migrations
        var migrationsAssembly = MigrationAssemblyConfiguration.GetMigrationAssemblyByProvider(options.DatabaseProvider);
        options.DatabaseMigrations.SetMigrationsAssemblies(migrationsAssembly);

        // Use production DbContexts and auth services.
        options.Testing.IsStaging = false;
    }
}