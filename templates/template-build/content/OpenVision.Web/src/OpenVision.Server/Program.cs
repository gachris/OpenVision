using Microsoft.EntityFrameworkCore;
using OpenVision.Server.Core.Configuration;
using OpenVision.Web.Core.Helpers;
using Serilog;

const string ConnectionStringName = "vision";

var configuration = StartupHelper.GetConfiguration<Program>(args);

Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

try
{
    DockerHelpers.ApplyDockerConfiguration(configuration);

    var builder = WebApplication.CreateBuilder(args);
    builder.ConfigureHostBuilder<Program>(args);

    var connectionString = builder.Configuration.GetConnectionString(ConnectionStringName)!;
    var databaseProviderType = builder.Configuration.GetValue("DatabaseProvider", DatabaseProviderType.SqlServer);
    var apiConfiguration = builder.Configuration.GetSection(nameof(ApiConfiguration)).Get<ApiConfiguration>()!;

    builder.AddServiceDefaults();
    builder.AddApiServiceDefaults(apiConfiguration, connectionString, databaseProviderType);

    var app = builder.Build();

    app.AddApplicationDefaults(apiConfiguration);

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