using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Helper class for configuring application startup settings.
/// </summary>
public class StartupHelper
{
    /// <summary>
    /// Retrieves the application configuration.
    /// </summary>
    /// <typeparam name="T">The type whose user secrets are being loaded (typically the entry point type).</typeparam>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>The application configuration.</returns>
    public static IConfiguration GetConfiguration<T>(string[] args) where T : class
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var isDevelopment = environment == Environments.Development;

        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("serilog.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"serilog.{environment}.json", optional: true, reloadOnChange: true);

        if (isDevelopment)
        {
            configurationBuilder.AddUserSecrets<T>(optional: true);
        }

        //var configuration = configurationBuilder.Build();
        //configuration.AddAzureKeyVaultConfiguration(configurationBuilder);

        configurationBuilder.AddCommandLine(args);
        configurationBuilder.AddEnvironmentVariables();

        return configurationBuilder.Build();
    }
}