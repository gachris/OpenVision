using OpenVision.Client.Core.Configuration;
using OpenVision.Web.Core.Helpers;
using Serilog;

var configuration = StartupHelper.GetConfiguration<Program>(args);

Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(configuration)
    .CreateLogger();

try
{
    DockerHelpers.ApplyDockerConfiguration(configuration);

    var builder = WebApplication.CreateBuilder(args);
    builder.ConfigureHostBuilder<Program>(args);

    var appConfiguration = builder.Configuration.GetSection(nameof(AppConfiguration)).Get<AppConfiguration>()!;
    var securityConfiguration = builder.Configuration.GetSection(nameof(SecurityConfiguration)).Get<SecurityConfiguration>()!;

    builder.AddServiceDefaults();
    builder.AddOpenVisionClientDefaults(appConfiguration);

    var app = builder.Build();

    app.UseOpenVisionClientDefaults(securityConfiguration);

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}