using OpenVision.Client.Core.Configuration;
using OpenVision.Client.Core.Middlewares;
using Serilog;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Helper class for configuring application startup settings.
/// </summary>
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
    /// Adds default services and configurations for an OpenVision client application.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance.</param>
    /// <returns>The configured <see cref="IHostApplicationBuilder"/> instance.</returns>
    public static IHostApplicationBuilder ConfigureOpenVisionClient(this WebApplicationBuilder builder)
    {
        // Retrieve configurations
        var appConfiguration = builder.Configuration
            .GetSection(nameof(AppConfiguration))
            .Get<AppConfiguration>()!;

        var oidcConfiguration = builder.Configuration
            .GetSection(nameof(OidcConfiguration))
            .Get<OidcConfiguration>()!;

        var openVisionServerOptionsSection = builder.Configuration
            .GetSection(nameof(OpenVisionApiOptions));

        // Register default services specific to your application.
        builder.AddServiceDefaults();

        // Register MediatR.
        builder.Services.AddDefaultMediatR();

        builder.Services.AddSingleton(appConfiguration);
        builder.Services.Configure<OpenVisionApiOptions>(openVisionServerOptionsSection);

        // Add application localization
        builder.Services.AddDefaultLocalization();

        // Add application services
        builder.Services.AddAutoMapperConfiguration();
        builder.Services.AddOpenVisionServices();

        // Add HttpContextAccessor
        builder.Services.AddHttpContextAccessor();

        // Add cookie policy
        builder.Services.AddDefaultCookiePolicy();

        // Add authentication
        builder.Services.AddIdentityServerAuthentication(oidcConfiguration);

        // Add application controllers
        builder.Services.AddControllersWithViews()
            .AddDefaultViewLocalization()
            .AddDataAnnotationsLocalization()
            .AddDefaultJsonOptions();

        return builder;
    }

    /// <summary>
    /// Configures default middleware and pipeline settings for the OpenVision client application.
    /// </summary>
    /// <param name="app">The <see cref="WebApplication"/> instance.</param>
    /// <returns>The configured <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder ConfigureOpenVisionClientPipeline(this WebApplication app)
    {
        // Retrieve configuration
        var securityConfiguration = app.Configuration
            .GetSection(nameof(SecurityConfiguration))
            .Get<SecurityConfiguration>()!;

        // Enable static files
        app.UseStaticFiles();

        // Use security headers
        app.UseSecurityHeaders(securityConfiguration.CspTrustedDomains);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        // Enable HTTPS redirection
        app.UseHttpsRedirection();

        // Enable routing
        app.UseRouting();

        // Use cookie policy
        app.UseCookiePolicy();

        // Use authentication middleware
        app.UseAuthentication();

        // Use token refresh middleware
        app.UseMiddleware<TokenRefreshMiddleware>();

        // Use authorization middleware
        app.UseAuthorization();

        // Map application controllers
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=home}/{action=index}/{id?}");

        return app;
    }
}
