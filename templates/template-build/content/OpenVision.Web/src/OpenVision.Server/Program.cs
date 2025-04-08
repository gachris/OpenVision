using OpenVision.Server.Core.Helpers;
using Serilog;

var configuration = ProgramHelper.GetConfiguration<Program>(args);

Log.Logger = new LoggerConfiguration().ReadFrom
    .Configuration(configuration)
    .CreateLogger();

try
{
    DockerHelpers.ApplyDockerConfiguration(configuration);

    var builder = WebApplication.CreateBuilder(args);
    builder.ConfigureHostBuilder<Program>(args);
    builder.ConfigureOpenVisionServer();

    var app = builder.Build();
    app.ConfigureOpenVisionServerPipeline();
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
