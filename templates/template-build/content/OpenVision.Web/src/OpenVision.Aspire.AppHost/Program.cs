using Microsoft.Extensions.Configuration;
using OpenVision.Aspire.AppHost.Configuration;

var builder = DistributedApplication.CreateBuilder(args);
var databaseProvider = builder.Configuration.GetSection("Parameters").GetValue<string>("databaseProvider")!;

IResourceBuilder<IResourceWithConnectionString> resourceBuilder;

var parameters = builder.Configuration
    .GetSection("Parameters");

var databaseProviderType = parameters
    .GetValue<DatabaseProviderType>(nameof(DatabaseProviderType));

switch (databaseProviderType)
{
    case DatabaseProviderType.MySql:
        {
            var resource = builder.AddMySql("mysql")
                .WithDataVolume("openvision-mysql-data");
            resourceBuilder = resource.AddDatabase("openvision");
        }
        break;
    case DatabaseProviderType.PostgreSQL:
        {
            var resource = builder.AddPostgres("postgresql")
                .WithDataVolume("openvision-postgresql-data");
            resourceBuilder = resource.AddDatabase("openvision");
        }
        break;
    default:
        {
            var resource = builder.AddSqlServer("sqlserver")
                .WithDataVolume("openvision-sqlserver-data");
            resourceBuilder = resource.AddDatabase("openvision");
        }
        break;
}

var databaseProviderTypeParameter = builder.AddParameter("DatabaseProviderType");
var usePooledDbContextParameter = builder.AddParameter("UsePooledDbContext");

builder.AddProject<Projects.OpenVision_Client>("openvision-client");
builder.AddProject<Projects.OpenVision_Server>("openvision-server")
       .WithEnvironment("DatabaseConfiguration:ProviderType", databaseProviderTypeParameter)
       .WithEnvironment("DatabaseConfiguration:UsePooledDbContext", usePooledDbContextParameter)
       .WithReference(resourceBuilder);

builder.Build().Run();