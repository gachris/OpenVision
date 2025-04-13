using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OpenVision.IdentityServer.Admin;
using OpenVision.IdentityServer.Admin.EntityFramework.Shared.DbContexts;
using OpenVision.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity;
using OpenVision.IdentityServer.Admin.EntityFramework.Shared.Helpers;
using Serilog;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Configuration.Configuration;
using Skoruba.Duende.IdentityServer.Shared.Configuration.Helpers;

const string SeedArgs = "/seed";
const string MigrateOnlyArgs = "/migrateonly";

var configuration = ProgramHelper.GetConfiguration<Program>(args);

Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

try
{
    DockerHelpers.ApplyDockerConfiguration(configuration);

    var builder = WebApplication.CreateBuilder(args);
    builder.ConfigureHostBuilder<Program>(args);

    builder.AddServiceDefaults();

    ProgramHelper.ConfigureServices(builder.Services, builder.Environment, builder.Configuration);

    var app = builder.Build();

    ProgramHelper.Configure(app);

    var migrationComplete = await ApplyDbMigrationsWithDataSeedAsync(args, configuration, app);
    if (await MigrateOnlyOperationAsync(args, app, migrationComplete)) return;

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
static async Task<bool> MigrateOnlyOperationAsync(string[] args, IHost host, bool migrationComplete)
{
    if (args.Any(x => x == MigrateOnlyArgs))
    {
        await host.StopAsync();

        if (!migrationComplete)
        {
            Environment.ExitCode = -1;
        }

        return true;
    }

    return false;
}

static async Task<bool> ApplyDbMigrationsWithDataSeedAsync(string[] args, IConfiguration configuration, IHost host)
{
    var applyDbMigrationWithDataSeedFromProgramArguments = args.Any(x => x == SeedArgs);
    if (applyDbMigrationWithDataSeedFromProgramArguments) args = args.Except(new[] { SeedArgs }).ToArray();

    var seedConfiguration = configuration.GetSection(nameof(SeedConfiguration)).Get<SeedConfiguration>();
    var databaseMigrationsConfiguration = configuration.GetSection(nameof(DatabaseMigrationsConfiguration))
        .Get<DatabaseMigrationsConfiguration>();

    return await DbMigrationHelpers
        .ApplyDbMigrationsWithDataSeedAsync<IdentityServerConfigurationDbContext, AdminIdentityDbContext,
            IdentityServerPersistedGrantDbContext, AdminLogDbContext, AdminAuditLogDbContext,
            IdentityServerDataProtectionDbContext, UserIdentity, UserIdentityRole>(host,
            applyDbMigrationWithDataSeedFromProgramArguments, seedConfiguration, databaseMigrationsConfiguration);
}
