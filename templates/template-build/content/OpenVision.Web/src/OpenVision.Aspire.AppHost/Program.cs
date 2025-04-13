using Microsoft.Extensions.Configuration;
using OpenVision.Aspire.AppHost.Configuration;

var builder = DistributedApplication.CreateBuilder(args);
var databaseProvider = builder.Configuration.GetSection("Parameters").GetValue<string>("databaseProvider")!;

IResourceBuilder<IResourceWithConnectionString> openVisionDataDbConnectionResourceBuilder;
IResourceBuilder<IResourceWithConnectionString> identityServerConfigurationDbConnectionResourceBuilder;
IResourceBuilder<IResourceWithConnectionString> identityServerPersistedGrantDbConnectionResourceBuilder;
IResourceBuilder<IResourceWithConnectionString> identityServerIdentityDbConnectionResourceBuilder;
IResourceBuilder<IResourceWithConnectionString> identityServerAdminLogDbConnectionResourceBuilder;
IResourceBuilder<IResourceWithConnectionString> identityServerAdminAuditLogDbConnectionResourceBuilder;
IResourceBuilder<IResourceWithConnectionString> identityServerDataProtectionDbConnectionResourceBuilder;

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

            openVisionDataDbConnectionResourceBuilder = resource.AddDatabase("DataDbConnection", "OpenVision");
            identityServerConfigurationDbConnectionResourceBuilder = resource.AddDatabase("ConfigurationDbConnection", "IdentityServerAdmin");
            identityServerPersistedGrantDbConnectionResourceBuilder = resource.AddDatabase("PersistedGrantDbConnection", "IdentityServerAdmin");
            identityServerIdentityDbConnectionResourceBuilder = resource.AddDatabase("IdentityDbConnection", "IdentityServerAdmin");
            identityServerAdminLogDbConnectionResourceBuilder = resource.AddDatabase("AdminLogDbConnection", "IdentityServerAdmin");
            identityServerAdminAuditLogDbConnectionResourceBuilder = resource.AddDatabase("AdminAuditLogDbConnection", "IdentityServerAdmin");
            identityServerDataProtectionDbConnectionResourceBuilder = resource.AddDatabase("DataProtectionDbConnection", "IdentityServerAdmin");
        }
        break;
    case DatabaseProviderType.PostgreSQL:
        {
            var resource = builder.AddPostgres("postgresql")
                .WithDataVolume("openvision-postgresql-data");

            openVisionDataDbConnectionResourceBuilder = resource.AddDatabase("DataDbConnection", "OpenVision");
            identityServerConfigurationDbConnectionResourceBuilder = resource.AddDatabase("ConfigurationDbConnection", "IdentityServerAdmin");
            identityServerPersistedGrantDbConnectionResourceBuilder = resource.AddDatabase("PersistedGrantDbConnection", "IdentityServerAdmin");
            identityServerIdentityDbConnectionResourceBuilder = resource.AddDatabase("IdentityDbConnection", "IdentityServerAdmin");
            identityServerAdminLogDbConnectionResourceBuilder = resource.AddDatabase("AdminLogDbConnection", "IdentityServerAdmin");
            identityServerAdminAuditLogDbConnectionResourceBuilder = resource.AddDatabase("AdminAuditLogDbConnection", "IdentityServerAdmin");
            identityServerDataProtectionDbConnectionResourceBuilder = resource.AddDatabase("DataProtectionDbConnection", "IdentityServerAdmin");
        }
        break;
    default:
        {
            var resource = builder.AddSqlServer("sqlserver")
                .WithDataVolume("openvision-sqlserver-data");

            openVisionDataDbConnectionResourceBuilder = resource.AddDatabase("DataDbConnection", "OpenVision");
            identityServerConfigurationDbConnectionResourceBuilder = resource.AddDatabase("ConfigurationDbConnection", "IdentityServerAdmin");
            identityServerPersistedGrantDbConnectionResourceBuilder = resource.AddDatabase("PersistedGrantDbConnection", "IdentityServerAdmin");
            identityServerIdentityDbConnectionResourceBuilder = resource.AddDatabase("IdentityDbConnection", "IdentityServerAdmin");
            identityServerAdminLogDbConnectionResourceBuilder = resource.AddDatabase("AdminLogDbConnection", "IdentityServerAdmin");
            identityServerAdminAuditLogDbConnectionResourceBuilder = resource.AddDatabase("AdminAuditLogDbConnection", "IdentityServerAdmin");
            identityServerDataProtectionDbConnectionResourceBuilder = resource.AddDatabase("DataProtectionDbConnection", "IdentityServerAdmin");
        }
        break;
}

var databaseProviderTypeParameter = builder.AddParameter("DatabaseProviderType");
var usePooledDbContextParameter = builder.AddParameter("UsePooledDbContext");

builder.AddProject<Projects.OpenVision_Client>("openvision-client");

builder.AddProject<Projects.OpenVision_Server>("openvision-server")
       .WithEnvironment("DatabaseConfiguration:ProviderType", databaseProviderTypeParameter)
       .WithEnvironment("DatabaseConfiguration:UsePooledDbContext", usePooledDbContextParameter)
       .WithReference(openVisionDataDbConnectionResourceBuilder);

builder.AddProject<Projects.OpenVision_IdentityServer_Admin_Api>("openvision-identityserver-admin-api")
       .WithReference(identityServerConfigurationDbConnectionResourceBuilder)
       .WithReference(identityServerPersistedGrantDbConnectionResourceBuilder)
       .WithReference(identityServerIdentityDbConnectionResourceBuilder)
       .WithReference(identityServerAdminLogDbConnectionResourceBuilder)
       .WithReference(identityServerAdminAuditLogDbConnectionResourceBuilder)
       .WithReference(identityServerDataProtectionDbConnectionResourceBuilder);

builder.AddProject<Projects.OpenVision_IdentityServer_Admin>("openvision-identityserver-admin")
       .WithReference(identityServerConfigurationDbConnectionResourceBuilder)
       .WithReference(identityServerPersistedGrantDbConnectionResourceBuilder)
       .WithReference(identityServerIdentityDbConnectionResourceBuilder)
       .WithReference(identityServerAdminLogDbConnectionResourceBuilder)
       .WithReference(identityServerAdminAuditLogDbConnectionResourceBuilder)
       .WithReference(identityServerDataProtectionDbConnectionResourceBuilder);

builder.AddProject<Projects.OpenVision_IdentityServer_STS_Identity>("openvision-identityserver-sts-identity")
       .WithReference(identityServerConfigurationDbConnectionResourceBuilder)
       .WithReference(identityServerPersistedGrantDbConnectionResourceBuilder)
       .WithReference(identityServerIdentityDbConnectionResourceBuilder)
       .WithReference(identityServerAdminLogDbConnectionResourceBuilder)
       .WithReference(identityServerAdminAuditLogDbConnectionResourceBuilder)
       .WithReference(identityServerDataProtectionDbConnectionResourceBuilder);

builder.Build().Run();
