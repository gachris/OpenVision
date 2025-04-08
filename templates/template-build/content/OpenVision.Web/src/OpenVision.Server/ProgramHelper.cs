using Microsoft.EntityFrameworkCore;
using OpenVision.Core.Configuration;
using OpenVision.Core.DataTypes;
using OpenVision.Server.Core.Configuration;
using OpenVision.Server.Core.Middlewares;
using Serilog;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Helper class for configuring application startup settings.
/// </summary>
internal static class ProgramHelper
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

        // Add environment variables and command-line arguments.
        builder.Configuration.AddEnvironmentVariables();
        builder.Configuration.AddCommandLine(args);

        // Configure Kestrel to not include the server header in responses.
        builder.WebHost.ConfigureKestrel(options => options.AddServerHeader = false);

        // Clear existing logging providers and add Serilog.
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();

        // Configure the host to use Serilog for logging.
        builder.Host.UseSerilog((hostContext, loggerConfig) =>
        {
            loggerConfig
                .ReadFrom.Configuration(hostContext.Configuration)
                // Enrich logs with the application name.
                .Enrich.WithProperty("ApplicationName", hostContext.HostingEnvironment.ApplicationName);
        });
    }

    /// <summary>
    /// Configures and registers OpenVision-related services and dependencies.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance.</param>
    /// <returns>The updated IHostApplicationBuilder instance.</returns>
    public static IHostApplicationBuilder ConfigureOpenVisionServer(this WebApplicationBuilder builder)
    {
        // Retrieve and bind configuration sections for various components.
        var databaseConfiguration = builder.Configuration
            .GetSection(nameof(DatabaseConfiguration))
            .Get<DatabaseConfiguration>()!;

        var corsConfiguration = builder.Configuration
            .GetSection(nameof(CorsConfiguration))
            .Get<CorsConfiguration>()!;

        var swaggerConfiguration = builder.Configuration
            .GetSection(nameof(SwaggerConfiguration))
            .Get<SwaggerConfiguration>()!;

        var oidcConfiguration = builder.Configuration
            .GetSection(nameof(OidcConfiguration))
            .Get<OidcConfiguration>()!;

        // Get the connection string using the connection name from the database configuration.
        var connectionString = builder.Configuration.GetConnectionString(databaseConfiguration.ConnectionName)!;

        // Register default services specific to your application.
        builder.AddServiceDefaults();

        // Register MediatR.
        builder.Services.AddDefaultMediatR();

        // Add GraphQL services, passing the flag indicating if a pooled DbContext is in use.
        builder.Services.AddGraphQL(databaseConfiguration.UsePooledDbContext);

        // Register AutoMapper, repositories, and core services.
        builder.Services.AddAutoMapperConfiguration();
        builder.Services.AddRepositories();
        builder.Services.AddOpenVisionCoreServices();
        builder.Services.AddCurrentUserService();

        // Register the DbContext, choosing between a pooled factory or a regular DbContext based on configuration.
        if (databaseConfiguration.UsePooledDbContext)
        {
            builder.Services.AddPooledDbContextFactory(connectionString, databaseConfiguration, options =>
            {
                options.UseLazyLoadingProxies();
            });
        }
        else
        {
            builder.Services.AddDbContext(connectionString, databaseConfiguration, options =>
            {
                options.UseLazyLoadingProxies();
            });
        }

        // Register the IHttpContextAccessor to allow services access to the current HTTP context.
        builder.Services.AddHttpContextAccessor();

        // Add authentication services (JWT and API key support).
        builder.Services.AddDefaultAuthentication()
            .AddDefaultJwtBearer(oidcConfiguration)
            .AddApiKeyScheme();

        // Register authorization policies.
        builder.Services.AddAuthorizationBuilder()
            .AddDefaultPolicy(oidcConfiguration);

        // Add CORS policies.
        builder.Services.AddDefaultCors(corsConfiguration);

        // Add API controllers with JSON options.
        builder.Services.AddDefaultControllers()
            .AddDefaultJsonOptions();

        // Register endpoints explorer and Swagger generation.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddDefaultSwaggerGen(swaggerConfiguration);

        // Configure the global image request builder settings.
        VisionSystemConfig.ImageRequestBuilder = new ImageRequestBuilder()
            .WithGrayscale()
            .WithGaussianBlur(new System.Drawing.Size(5, 5), 0);

        return builder;
    }

    /// <summary>
    /// Configures the HTTP request pipeline for the OpenVision application.
    /// </summary>
    /// <param name="app">The WebApplication instance.</param>
    /// <returns>The updated IApplicationBuilder instance.</returns>
    public static IApplicationBuilder ConfigureOpenVisionServerPipeline(this WebApplication app)
    {
        // Bind the database configuration and Swagger configuration.
        var databaseConfiguration = app.Configuration
            .GetSection(nameof(DatabaseConfiguration))
            .Get<DatabaseConfiguration>()!;

        var swaggerConfiguration = app.Configuration
            .GetSection(nameof(SwaggerConfiguration))
            .Get<SwaggerConfiguration>()!;

        // Configure WebSocket options for the application.
        var webSocketOptions = new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromMinutes(2)
        };

        // Enable WebSocket support.
        app.UseWebSockets(webSocketOptions);

        // Configure forwarded headers for proxy scenarios.
        app.UseDefaultsForwardHeaders();

        // In development, enable the developer exception page.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Configure a global exception handler middleware.
        app.ConfigureExceptionHandler();

        // Enable Cross-Origin Resource Sharing (CORS).
        app.UseCors();

        // Enforce HTTPS redirection.
        app.UseHttpsRedirection();

        // Set up routing for the application.
        app.UseRouting();

        // Enable authentication middleware.
        app.UseAuthentication();

        // Use custom challenge middleware for authentication challenges.
        app.UseMiddleware<ChallengeMiddleware>();

        // Enable authorization middleware.
        app.UseAuthorization();

        // Map API controllers.
        app.MapControllers();

        // Map GraphQL endpoints.
        app.MapGraphQL();

        // Configure Swagger and Swagger UI.
        app.UseSwagger(swaggerConfiguration);

        // Apply any pending database migrations.
        app.MigrateDatabase(databaseConfiguration.UsePooledDbContext);

        return app;
    }
}
