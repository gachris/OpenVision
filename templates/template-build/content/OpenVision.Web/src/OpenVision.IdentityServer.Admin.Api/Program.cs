using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using OpenVision.IdentityServer.Admin.Api;
using Serilog;
using Skoruba.Duende.IdentityServer.Shared.Configuration.Helpers;

var configuration = ProgramHelper.GetConfiguration<Program>(args);

Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
try
{
    DockerHelpers.ApplyDockerConfiguration(configuration);

    var builder = WebApplication.CreateBuilder(args);
    builder.ConfigureHostBuilder<Program>(args);

    builder.AddServiceDefaults();
    ProgramHelper.ConfigureServices(builder.Services, builder.Configuration);

    var app = builder.Build();

    ProgramHelper.Configure(app, builder.Environment, app.Configuration);

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
