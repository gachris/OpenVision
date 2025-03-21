using OpenVision.Web.Core.Helpers;
using OpenVision.Client.Core.Configuration;
using Serilog;

var configuration = StartupHelper.GetConfiguration<Program>(args);

Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

try
{
    DockerHelpers.ApplyDockerConfiguration(configuration);

    var builder = WebApplication.CreateBuilder(args);
    builder.ConfigureHostBuilder<Program>(args);

    builder.AddServiceDefaults();

    var appConfiguration = builder.Configuration.GetSection(nameof(AppConfiguration)).Get<AppConfiguration>()!;

    builder.Services.AddAppServices(builder.Configuration)
                    .AddAppLocalization()
                    .AddHttpContextAccessor()
                    .AddAppCookiePolicy()
                    .AddAppAuthentication(appConfiguration)
                    .AddAppControllers();

    var app = builder.Build();

    app.ConfigureDevelopmentEnvironment()
       .UseHttpsRedirection()
       .UseStaticFiles()
       .UseRouting()
       .UseCookiePolicy()
       .UseAuthentication()
       .UseAuthorization();

    app.MapAppControllerRoute();

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