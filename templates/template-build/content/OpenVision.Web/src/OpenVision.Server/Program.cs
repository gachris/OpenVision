using Microsoft.EntityFrameworkCore;
using OpenVision.Core.Configuration;
using OpenVision.Core.DataTypes;
using OpenVision.Server.Core.Configuration;
using OpenVision.Web.Core.Helpers;
using Serilog;

const string ConnectionStringName = "vision";

var configuration = StartupHelper.GetConfiguration<Program>(args);

Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(configuration)
    .CreateLogger();

VisionSystemConfig.ImageRequestBuilder = new ImageRequestBuilder()
    .WithGrayscale()
    .WithGaussianBlur(new System.Drawing.Size(5, 5), 0);

try
{
    DockerHelpers.ApplyDockerConfiguration(configuration);

    var builder = WebApplication.CreateBuilder(args);
    builder.ConfigureHostBuilder<Program>(args);

    var connectionString = builder.Configuration.GetConnectionString(ConnectionStringName)!;
    var databaseProviderType = builder.Configuration.GetValue("DatabaseProvider", DatabaseProviderType.SqlServer);
    var apiConfiguration = builder.Configuration.GetSection(nameof(ApiConfiguration)).Get<ApiConfiguration>()!;

    builder.AddServiceDefaults();
    builder.AddOpenVisionServerDefaults(apiConfiguration, connectionString, databaseProviderType);

    var app = builder.Build();

    app.UseOpenVisionServerDefaults(apiConfiguration);

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